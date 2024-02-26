using System.Collections.Generic;
using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.Networking;
using Stack = Pulumi.Stack;

namespace PoC.Deployment.Network;

public class NetworkStack : Stack
{
  private IDictionary<Environment, string[]> Prefixes = new Dictionary<Environment, string[]>()
  {
    {
      Environment.Dev, new[]
      {
        "10.0.0.0/16"
      }
    }
  };

  private IDictionary<Environment, IDictionary<string, string>> Subnets = new Dictionary<Environment, IDictionary<string, string>>()
  {
    {
      Environment.Dev, new Dictionary<string, string>()
      {
        { "subnet1", "10.0.0.0/18" },
        { "subnet2", "10.0.64.0/18"},
        { "subnet3", "10.0.128.0/18" },
        { "subnet4", "10.0.192.0/18" }
      }
    }
  };

  public NetworkStack()
  {
    using (Context.Initialize(Subscription.Dev, Environment.Dev, Region.PolandCentral, "network"))
    {
      var rgp = new ResourceGroupBuilder()
        .DisableProtection()
        .Build();

      var nsg = new NetworkSecurityGroupBuilder()
        .In(rgp)
        .DisableProtection()
        .Build();

      var routeTable = new RouteTableBuilder()
        .In(rgp)
        .DisableProtection()
        .Build();

      var network = new VirtualNetworkBuilder()
        .In(rgp)
        .WithAddressPrefixes(Prefixes[Context.Current.Environment])
        .WithSubnets(Subnets[Context.Current.Environment])
        .WithRouteTable(routeTable.Resource)
        .WithNetworkSecurityGroup(nsg.Resource)
        .DisableProtection()
        .Build();
    }
  }
}
