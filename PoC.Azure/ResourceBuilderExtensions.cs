using PoC.Azure.Deployment;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure;

public static class ResourceBuilderExtensions
{
  public static TResourceBuilder WithName<TResourceBuilder>(this TResourceBuilder builder, string name)
    where TResourceBuilder : ResourceBuilder
  {
    builder.Name = name;
    return builder;
  }

  public static TResourceBuilder WithResourceGroupName<TResourceBuilder>(this TResourceBuilder builder, string name)
    where TResourceBuilder : ResourceBuilder
  {
    builder.ResourceGroupName = name;
    return builder;
  }

  public static TResourceBuilder In<TResourceBuilder>(this TResourceBuilder builder, ResourceGroup resourceGroup)
    where TResourceBuilder : ResourceBuilder
  {
    builder.ResourceGroup = resourceGroup;
    return builder;
  }

  public static TResourceBuilder WithLocation<TResourceBuilder>(this TResourceBuilder builder, Region location)
    where TResourceBuilder : ResourceBuilder
  {
    builder.Location = location;
    return builder;
  }
}
