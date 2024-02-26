using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;

namespace PoC.Azure.Storage;

public static class StorageAccountExtensions
{
  public static (ResourceGroup ResourceGroup, StorageAccount Resource) AddPrimaryKeyToKeyVault(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key)
  {
    return storage.AddKeyToKeyVaultInternal(vault, key, true);
  }

  public static (ResourceGroup ResourceGroup, StorageAccount Resource) AddSecondaryKeyToKeyVault(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key)
  {
    return storage.AddKeyToKeyVaultInternal(vault, key, false);
  }

  private static (ResourceGroup ResourceGroup, StorageAccount Resource) AddKeyToKeyVaultInternal(this (ResourceGroup ResourceGroup, StorageAccount Resource) storage, (Output<string> ResourceGroup, Output<string> Vault) vault, string key, bool primary)
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

      });

    return storage;
  }
}
