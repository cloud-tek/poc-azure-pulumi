using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.KeyVault;
using Pulumi;
using Stack = Pulumi.Stack;

namespace PoC.Deployment.KeyVault;

public class KeyVaultStack : Stack
{
  public KeyVaultStack()
  {
    var config = new Config();

    using (Context.Initialize(Pulumi.Deployment.Instance.StackName.ToEnvironment(), Region.PolandCentral, "secret"))
    {
      var rgp = new ResourceGroupBuilder()
        .DisableProtection()
        .Build();

      var kv = new KeyVaultBuilder()
        .In(rgp)
        .InTenant(config.Require("azure.tenantId"))
        .WithPermissiveAccessPolicy("2b40e175-72b6-4243-a20d-869b15a4605c")
        .WithSoftDelete(false)
        .DisableProtection()
        .Build();

      Name = kv.Resource.Name;
      ResourceGroupName = rgp.Name;
    }
  }

  [Output] public Output<string> Name { get; set; }
  [Output] public Output<string> ResourceGroupName { get; set; }
}
