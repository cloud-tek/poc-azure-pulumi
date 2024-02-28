using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Resources;
using NetworkSecurityGroupArgs = Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs;
using RouteTableArgs = Pulumi.AzureNative.Network.Inputs.RouteTableArgs;

namespace PoC.Azure.Networking;

public static class VirtualNetworkExtensions
{
  public static Output<string> AddSubnet(
    this (ResourceGroup ResourceGroup, VirtualNetwork Resource) network,
    string name,
    string prefix,
    (ResourceGroup ResourceGroup, RouteTable Resource) routeTable,
    (ResourceGroup ResourceGroup, NetworkSecurityGroup Resource) nsg,
    bool protect = true)
  {
    var subnet = new Subnet($"{name}-{prefix}", new SubnetArgs()
    {
      SubnetName = name,
      AddressPrefix = prefix,
      ResourceGroupName = network.ResourceGroup.Name,
      VirtualNetworkName = network.Resource.Name,
      NetworkSecurityGroup = new NetworkSecurityGroupArgs()
      {
        Id = nsg.Resource.Id
      },
      RouteTable = new RouteTableArgs()
      {
        Id = routeTable.Resource.Id
      },
      PrivateEndpointNetworkPolicies = "Disabled", // Disabling the policy in order to allow Private Endpoints
      PrivateLinkServiceNetworkPolicies = "Enabled",
    }, new CustomResourceOptions()
    {
      Protect = protect
    });

    return subnet.Id;
  }
}
