using System.Collections.Generic;
using PoC.Azure.Deployment;

namespace PoC.Deployment.Network;

public partial class NetworkStack
{
  private readonly IDictionary<Environment, IDictionary<Region, string[]>> Prefixes
      = new Dictionary<Environment, IDictionary<Region, string[]>>()
    {
      {
        Environment.Dev, new Dictionary<Region, string[]>()
        {
          {
            Region.PolandCentral, new[]
            {
              "10.0.0.0/16"
            }
          },
          {
            Region.SwedenCentral, new[]
            {
              "10.1.0.0/16"
            }
          }
        }
      }
    };

    private IDictionary<Environment, IDictionary<Region, IDictionary<string, string>>> Subnets
      = new Dictionary<Environment, IDictionary<Region, IDictionary<string, string>>>()
    {
      {
        Environment.Dev, new Dictionary<Region, IDictionary<string, string>>()
        {
          {
            Region.PolandCentral , new Dictionary<string, string>()
            {
              { "subnet1", "10.0.0.0/18" },
              { "subnet2", "10.0.64.0/18"},
              { "subnet3", "10.0.128.0/18" },
              { "private-endpoints", "10.0.192.0/18" }
            }
          },
          {
            Region.SwedenCentral , new Dictionary<string, string>()
            {
              { "subnet1", "10.1.0.0/18" },
              { "subnet2", "10.1.64.0/18"},
              { "subnet3", "10.1.128.0/18" },
              { "private-endpoints", "10.1.192.0/18" }
            }
          }
        }
      }
    };
}
