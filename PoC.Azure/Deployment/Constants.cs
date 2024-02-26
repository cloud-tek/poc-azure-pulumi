using System.Collections;
using System.Collections.ObjectModel;

namespace PoC.Azure.Deployment;

internal static class Constants
{
  internal static IDictionary<Region, string> RegionAbbreviations = new ReadOnlyDictionary<Region, string>(new Dictionary<Region, string>() {
    { Region.WestEurope,  "euw" },
    { Region.NorthEurope,  "eun" },
    { Region.PolandCentral, "plc" }
  });

  internal static IDictionary<ResourceType, string> ResourceAbbreviations =
    new ReadOnlyDictionary<ResourceType, string>( new Dictionary<ResourceType, string>()
    {
      { ResourceType.Aks, "aks"},
      // ApplicationInsights,
      // AppServicePlan,
      // AutomationAccount,
      // BatchAccount,
      // ContainerInstance,
      // ContainerRegistry,
      // CosmosDb,
      // EventHub,
      // EventGrid,
      // FunctionApp,
      // ManagedIdentity,
      { ResourceType.KeyVault, "kv"},
      // LogAnalytics,
      { ResourceType.RouteTable, "udr"},
      // MsSql,
      // MsSqlDatabase,
      // MsSqlElasticPool,
      // MsSqlServer,
      // NetworkProfile,
      { ResourceType.NetworkSecurityGroup, "nsg" },
      // PgSql,
      // PgSqlFlexibleServer,
      // PrivateDnsZone,
      // PrivateEndpoint,
      { ResourceType.ResourceGroup, "rgp" },
      // RouteTable,
      // ServiceBus,
      { ResourceType.StorageAccount , String.Empty},
      // SubnetDelegation,
      // VirtualMachine,
      { ResourceType.VirtualNetwork, "vnet" },
    });
}
