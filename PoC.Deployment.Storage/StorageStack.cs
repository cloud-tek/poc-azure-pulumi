using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.Storage;
using PoC.Deployment.KeyVault;
using Pulumi;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

namespace PoC.Deployment.Storage;

public class StorageStack : Stack
{
  public StorageStack()
  {
    var kv = StackReference<KeyVaultStack>.Create(Pulumi.Deployment.Instance.StackName);
    //var kvRef = new StackReference($"organization/PoC.Deployment.KeyVault/{Pulumi.Deployment.Instance.StackName}");
    //'kvRef.RequireOutput("ResourceGroupName").Apply(x => x.ToString())!,
    // kvRef.RequireOutput("Name").Apply(x => x.ToString())!);

    var vaultRef = (
      ResourceGroup: kv.RequireOutput(x => x.ResourceGroupName),
      Resource: kv.RequireOutput(x => x.Name));

    // Access an output from the other stack
    //var databasePassword = otherStack.GetOutputAsync<string>("DatabasePassword");

    using (Context.Initialize(Environment.Dev, Region.PolandCentral, "data"))
    {
      var rgp = new ResourceGroupBuilder()
        .DisableProtection()
        .Build();

      var storage = new StorageAccountBuilder()
        .In(rgp)
        .WithSKU(SkuName.Standard_LRS)
        .DisableProtection()
        .Build();

      storage
        .AddPrimaryKeyToKeyVault(vaultRef!, "data-PrimaryKey")
        .AddSecondaryKeyToKeyVault(vaultRef!, "data-SecondaryKey");
    }
  }
}
