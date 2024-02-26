using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure;

public class ResourceGroupBuilder : ResourceBuilder<ResourceGroup>
{
  public ResourceGroupBuilder() : base(ResourceType.ResourceGroup)
  {
    ResourceGroupName = GenerateDefaultResourceName(ResourceType);
  }

  public override ResourceGroup Build()
  {
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
}
