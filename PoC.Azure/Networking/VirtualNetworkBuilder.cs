using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.Resources;
using NetworkSecurityGroupArgs = Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs;
using RouteTableArgs = Pulumi.AzureNative.Network.Inputs.RouteTableArgs;
using SubnetArgs = Pulumi.AzureNative.Network.SubnetArgs;

namespace PoC.Azure.Networking;

public class VirtualNetworkBuilder : AzureResourceBuilder<VirtualNetwork>
{
  public VirtualNetworkBuilder() : base(ResourceType.VirtualNetwork)
  { }

  private string[] AddressPrefixes = default!;

  private IDictionary<string, string> SubNets = default!;

  private RouteTable RouteTable = null;
  private NetworkSecurityGroup Nsg = null;

  public VirtualNetworkBuilder WithAddressPrefixes(string[] addressPrefixes)
  {
    AddressPrefixes = addressPrefixes;
    return this;
  }

  public VirtualNetworkBuilder WithSubnets(IDictionary<string, string> subnets)
  {
    SubNets = subnets;
    return this;
  }

  public VirtualNetworkBuilder WithNetworkSecurityGroup(NetworkSecurityGroup nsg)
  {
    Nsg = nsg;
    return this;
  }

  public VirtualNetworkBuilder WithRouteTable(RouteTable table)
  {
    RouteTable = table;
    return this;
  }

  public override (ResourceGroup ResourceGroup, VirtualNetwork Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var virtualNetwork = new VirtualNetwork(Name, new VirtualNetworkArgs()
    {
      AddressSpace = new AddressSpaceArgs()
      {
        AddressPrefixes = AddressPrefixes ?? throw new InvalidOperationException()
      },
      Location = Context.Current.Location.ToString(),
      VirtualNetworkName = Name,
      ResourceGroupName = resourceGroup.Name,
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    SubNets.ForEach(kvp =>
    {
      var subnet = new Subnet(kvp.Key, new SubnetArgs()
      {
        SubnetName = kvp.Key,
        AddressPrefix = kvp.Value,
        ResourceGroupName = resourceGroup.Name,
        VirtualNetworkName = virtualNetwork.Name,
        NetworkSecurityGroup = Nsg != null ? new NetworkSecurityGroupArgs()
        {
          Id = Nsg.Id
        } : null,
        RouteTable = RouteTable != null ? new RouteTableArgs()
        {
          Id = RouteTable.Id
        } : null
      }, new CustomResourceOptions()
      {
        Protect = Protect
      });
    });

    return (resourceGroup, virtualNetwork);
  }
}
