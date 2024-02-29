using PoC.Azure.Deployment;

namespace PoC.Azure;

public static class RegionExtensions
{
  public static string Abbreviate(this Region region)
  {
    return Constants.RegionAbbreviations[region];
  }
}
