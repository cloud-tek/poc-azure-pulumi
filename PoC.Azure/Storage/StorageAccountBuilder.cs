using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace PoC.Azure.Storage;

public class StorageAccountBuilder : AzureResourceBuilder<StorageAccount>
{
  private SkuName Sku { get; set; }
  private PublicNetworkAccess PublicNetworkAccess { get; set; } = PublicNetworkAccess.Disabled;

  public StorageAccountBuilder() : base(ResourceType.StorageAccount)
  {
  }

  public StorageAccountBuilder WithSKU(SkuName sku)
  {
    Sku = sku;
    return this;
  }

  public StorageAccountBuilder EnablePublicNetworkAccess()
  {
    this.PublicNetworkAccess = PublicNetworkAccess.Enabled;
    return this;
  }

  public override (ResourceGroup ResourceGroup, StorageAccount Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var managementLockByScope = new ManagementLockByScope("managementLockByScope", new()
    {
      Level = "CanNotDelete", // CanNotDelete | ReadOnly
      LockName = "lock",
      Scope = resourceGroup.Id
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });


    // Create an Azure resource (Storage Account)
    var storageAccount = new StorageAccount(Name, new StorageAccountArgs
    {
      AccountName = Name,
      ResourceGroupName = resourceGroup.Name,
      Sku = new SkuArgs
      {
        Name = Sku
      },
      Kind = Kind.StorageV2,
      PublicNetworkAccess = PublicNetworkAccess,
      EnableHttpsTrafficOnly = true,
      AllowBlobPublicAccess = false,
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    // https://www.pulumi.com/registry/packages/azure-native/api-docs/storage/managementpolicy/
    // https://www.pulumi.com/registry/packages/azure-native/api-docs/network/privateendpoint/

    return (resourceGroup, storageAccount);
  }
}
