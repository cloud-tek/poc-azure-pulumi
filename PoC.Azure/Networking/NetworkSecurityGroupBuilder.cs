using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure.Networking;

public class NetworkSecurityGroupBuilder : AzureResourceBuilder<NetworkSecurityGroup>
{
  public NetworkSecurityGroupBuilder(bool protect = true) : base(ResourceType.NetworkSecurityGroup, protect)
  {
  }

  public override (ResourceGroup ResourceGroup, NetworkSecurityGroup Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var nsg = new NetworkSecurityGroup(Name, new NetworkSecurityGroupArgs()
    {
      NetworkSecurityGroupName = Name,
      ResourceGroupName = resourceGroup.Name,
      Location = Context.Current.Location.ToString().ToLowerInvariant()
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    return (resourceGroup, nsg);
  }
}
