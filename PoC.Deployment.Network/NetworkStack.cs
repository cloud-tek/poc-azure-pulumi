using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PoC.Azure;
using PoC.Azure.Deployment;
using PoC.Azure.Networking;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Environment = PoC.Azure.Deployment.Environment;
using Stack = Pulumi.Stack;

namespace PoC.Deployment.Network;

public partial class NetworkStack : Stack
{
  public NetworkStack()
  {
    const bool protect = false;
    var outputs = new Dictionary<Region, Output<string>>();

    using (Context.Initialize(Pulumi.Deployment.Instance.StackName.ToEnvironment(), Region.PolandCentral, "network"))
    {
      var rgp = new ResourceGroupBuilder(protect: protect)
        .Build();

      CreateNetwork(rgp, (id) => { outputs[Context.Current.Location] = id; }, protect: protect);

      using (Context.In(Region.SwedenCentral))
      {
        CreateNetwork(rgp, (id) => { outputs[Context.Current.Location] = id; }, protect: protect);
      }
    }

    PrivateEndpointSubnets = Output.All(outputs.Values).Apply(output =>
    {
      var result = new Dictionary<string, object>();
      Enum.GetValues<Region>().ForEach(value =>
      {
        var o = output.SingleOrDefault(x => x.Contains(value.Abbreviate()));
        if (!string.IsNullOrEmpty(o))
        {
          result[value.Abbreviate()] = o;
        }
      });

      return result.ToImmutableDictionary();
    });
  }

  private void CreateNetwork(ResourceGroup rgp, Action<Output<string>> output, bool protect)
  {
    var nsg = new NetworkSecurityGroupBuilder(protect: protect)
      .In(rgp)
      .Build();

    var routeTable = new RouteTableBuilder(protect: protect)
      .In(rgp)
      .Build();

    var network = new VirtualNetworkBuilder(protect: protect)
      .In(rgp)
      .WithAddressPrefixes(Prefixes[Context.Current.Environment][Context.Current.Location])
      .Build();

    Subnets[Context.Current.Environment][Context.Current.Location].ForEach(kvp =>
    {
      var id = network.AddSubnet(kvp.Key, kvp.Value, routeTable, nsg, protect: protect);
      if (kvp.Key == "private-endpoints")
      {
        output(id);
      }
    });
  }

  [Output] public Output<ImmutableDictionary<string, object>> PrivateEndpointSubnets { get; set; } = default!;
  /*
    System.InvalidOperationException: [Output] PoC.Deployment.Network.NetworkStack.PrivateEndpointSubnets contains invalid type
    System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e],[System.String, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]:
      The only generic types allowed are ImmutableArray<...> and ImmutableDictionary<string, ...>
      at void Pulumi.Serialization.Converter.CheckTargetType(string context, Type targetType, HashSet<Type> seenTypes)
      at ImmutableDictionary<string, IOutputCompletionSource> Pulumi.Serialization.OutputCompletionSource.InitializeOutputs(Resource resource)
   */
}
