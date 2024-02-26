using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure.Networking;

public class RouteTableBuilder : AzureResourceBuilder<RouteTable>
{
  public RouteTableBuilder() : base(ResourceType.RouteTable)
  { }
  public override (ResourceGroup ResourceGroup, RouteTable Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var table = new RouteTable(Name, new RouteTableArgs()
    {
        RouteTableName = Name,
        ResourceGroupName = resourceGroup.Name,
        Location = Context.Current.Location.ToString().ToLowerInvariant()
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    return (resourceGroup, table);
  }
}
