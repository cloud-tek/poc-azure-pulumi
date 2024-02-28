using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.Storage;
using PoC.Deployment.KeyVault;
using PoC.Deployment.Network;
using Pulumi;
using Pulumi.AzureNative.Storage;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

namespace PoC.Deployment.Storage;

public class StorageStack : Stack
{
  public StorageStack()
  {
    //var kvRef = new StackReference($"organization/PoC.Deployment.KeyVault/{Pulumi.Deployment.Instance.StackName}");
    //'kvRef.RequireOutput("ResourceGroupName").Apply(x => x.ToString())!,
    // kvRef.RequireOutput("Name").Apply(x => x.ToString())!);
    var kv = StackReference<KeyVaultStack>.Create(Pulumi.Deployment.Instance.StackName);
    var network = StackReference<NetworkStack>.Create(Pulumi.Deployment.Instance.StackName);

    var vaultRef = (
      ResourceGroup: kv.RequireOutput(x => x.ResourceGroupName),
      Resource: kv.RequireOutput(x => x.Name));

    const bool protect = true;
    using (Context.Initialize(Environment.Dev, Region.PolandCentral, "data"))
    {
      var rgp = new ResourceGroupBuilder()
        .SetProtection(protect)
        .Build();

      var storage = new StorageAccountBuilder()
        .In(rgp)
        .WithSKU(SkuName.Standard_LRS)

        .SetProtection(protect)
        .Build();

      storage
        .AddPrimaryKeyToKeyVault(vaultRef!, "data-PrimaryKey", protect: protect)
        .AddSecondaryKeyToKeyVault(vaultRef!, "data-SecondaryKey", protect: protect);

      storage.AddBlobPrivateEndpoint(network.RequireOutput(x => x.PrivateEndpointSubnets)
        .Apply(output => output[Context.Current.Location.Abbreviate()].ToString()!), protect: protect);

      using (Context.In(Region.SwedenCentral))
      {
        storage.AddBlobPrivateEndpoint(network.RequireOutput(x => x.PrivateEndpointSubnets)
          .Apply(output => output[Context.Current.Location.Abbreviate()].ToString()!), protect: protect);
      }
    }
  }
}
