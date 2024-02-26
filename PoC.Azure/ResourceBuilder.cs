using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure;

public abstract class ResourceBuilder
{
  internal Region Location { get; set; } = default!;
  internal string ResourceGroupName { get; set; } = default!;
  internal ResourceGroup ResourceGroup { get; set; } = default!;

  internal string Name { get; set; } = default!;

  internal bool Protect { get; set; } = true;

  protected ResourceGroup GetResourceGroup()
  {
    if (ResourceGroup != null)
      return ResourceGroup;

    if (ResourceGroupName == null)
      throw new InvalidOperationException(
        $"No resource group provided. Reference a resource group using {nameof(ResourceBuilderExtensions.In)} or use {nameof(ResourceBuilderExtensions.WithResourceGroupName)}");

    var resourceGroup = new ResourceGroup(ResourceGroupName, new ResourceGroupArgs()
    {
      Location = Context.Current.Location.ToString().ToLowerInvariant(),
      ResourceGroupName = ResourceGroupName,
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    return resourceGroup;
  }

  protected static string GenerateDefaultResourceName(ResourceType resourceType)
  {
    var result = Constants.ResourceAbbreviations[resourceType].IsNullOrEmpty()
      ? $"{Constants.RegionAbbreviations[Context.Current.Location]}-{Context.Current.Module}-{Context.Current.Environment.ToString().ToLowerInvariant()}"
      : $"{Constants.RegionAbbreviations[Context.Current.Location]}-{Context.Current.Module}-{Context.Current.Environment.ToString().ToLowerInvariant()}-{Constants.ResourceAbbreviations[resourceType].ToLowerInvariant()}";

    if(resourceType == ResourceType.StorageAccount)
      result = result.Replace("-", "0");

    return result;
  }
}

public abstract class ResourceBuilder<TResource> : ResourceBuilder
  where TResource : CustomResource
{
  protected ResourceBuilder(ResourceType resourceType)
  {
    ResourceType = resourceType;
    Name = GenerateDefaultResourceName(resourceType);
  }
  public abstract TResource Build();
  protected ResourceType ResourceType { get; init; }
}

public abstract class AzureResourceBuilder<TResource> : ResourceBuilder
  where TResource : CustomResource
{
  protected AzureResourceBuilder(ResourceType resourceType)
  {
    ResourceType = resourceType;
    Name = GenerateDefaultResourceName(ResourceType);
  }
  public abstract (ResourceGroup ResourceGroup, TResource Resource) Build();
  protected ResourceType ResourceType { get; init; }
}
