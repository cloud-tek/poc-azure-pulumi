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

    const bool protect = true;
    using (Context.Initialize(Pulumi.Deployment.Instance.StackName.ToEnvironment(), Region.PolandCentral, "secret"))
    {
      var rgp = new ResourceGroupBuilder(protect: protect)
        .Build();

      var kv = new KeyVaultBuilder(protect: protect)
        .In(rgp)
        .InTenant(config.Require("azure.tenantId"))
        .WithPermissiveAccessPolicy("2b40e175-72b6-4243-a20d-869b15a4605c")
        .WithSoftDelete(false)
        .Build();

      Name = kv.Resource.Name;
      ResourceGroupName = rgp.Name;
    }
  }

  [Output] public Output<string> Name { get; set; }
  [Output] public Output<string> ResourceGroupName { get; set; }
}
