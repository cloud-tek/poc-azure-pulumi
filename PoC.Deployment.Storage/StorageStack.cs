using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.Storage;
using Pulumi;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

namespace PoC.Deployment.Storage;

public class StorageStack : Stack
{
  public StorageStack()
  {
    var kvRef = new StackReference($"organization/PoC.Deployment.KeyVault/{Pulumi.Deployment.Instance.StackName}");
    var vaultRef = (
      ResourceGroup: kvRef.RequireOutput("ResourceGroupName").Apply(x => x.ToString())!,
      Resource: kvRef.RequireOutput("Name").Apply(x => x.ToString())!);

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
