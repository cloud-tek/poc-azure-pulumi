using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using SubnetArgs = Pulumi.AzureNative.Network.Inputs.SubnetArgs;

namespace PoC.Azure.Storage;

public static class StorageAccountExtensions
{
  public static (ResourceGroup ResourceGroup, StorageAccount Resource) AddPrimaryKeyToKeyVault(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key, bool protect = true)
  {
    return storage.AddKeyToKeyVaultInternal(vault, key, true, protect);
  }

  public static (ResourceGroup ResourceGroup, StorageAccount Resource) AddSecondaryKeyToKeyVault(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key, bool protect = true)
  {
    return storage.AddKeyToKeyVaultInternal(vault, key, false, protect);
  }

  private static (ResourceGroup ResourceGroup, StorageAccount Resource) AddKeyToKeyVaultInternal(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key, bool primary, bool protect = true)
  {
    var secret = new Secret(
      key,
      new SecretArgs()
      {
        SecretName = key,
        VaultName = vault.Vault,
        ResourceGroupName = vault.ResourceGroup,
        Properties = new SecretPropertiesArgs()
        {
          Value = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs()
          {
            ResourceGroupName = storage.ResourceGroup.Name,
            AccountName = storage.Resource.Name
          }).Apply(x => x.Keys[primary ? 0 : 1].Value)
        }
      },
      new CustomResourceOptions()
      {
        Protect = protect
      });

    return storage;
  }

  public static (ResourceGroup ResourceGroup, StorageAccount Resource) AddBlobPrivateEndpoint(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, Output<string> subnetId, bool protect = true)
  {
    var name = $"{ResourceBuilder.GenerateDefaultResourceName(ResourceType.StorageAccount)}";
    var privateEndpoint = new PrivateEndpoint(name, new PrivateEndpointArgs
    {
      Location = Context.Current.Location.ToString().ToLowerInvariant(),
      PrivateEndpointName = $"{name}-pe",
      ResourceGroupName = storage.ResourceGroup.Name,
      Subnet = new SubnetArgs { Id = subnetId }, // Associate with our Private Endpoint Subnet
      PrivateLinkServiceConnections =
      {
        new PrivateLinkServiceConnectionArgs
        {
          Name = $"{name}-plink",
          PrivateLinkServiceId = storage.Resource.Id,
          GroupIds = { "blob" }, // Connect to the Storage Account's Blob service
        }
      },
      CustomNetworkInterfaceName = $"{name}-nic"
    }, new CustomResourceOptions()
    {
      Protect = protect
    });

    return storage;
  }
}
