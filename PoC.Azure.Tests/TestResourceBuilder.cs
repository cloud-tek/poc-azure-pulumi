using PoC.Azure.Deployment;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;

namespace PoC.Azure.Tests;

public class TestResourceBuilder : AzureResourceBuilder<StorageAccount>
{
  public TestResourceBuilder() : base(ResourceType.StorageAccount, false)
  {

  }
  public override (ResourceGroup, StorageAccount) Build()
  {
    var resourceGroup = GetResourceGroup();
    throw new NotImplementedException();
  }
}
