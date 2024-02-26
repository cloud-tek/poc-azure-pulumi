using PoC.Azure.Deployment;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.Resources;

namespace PoC.Azure.KeyVault;

public class KeyVaultBuilder : AzureResourceBuilder<Vault>
{
  private SkuName _sku = SkuName.Standard;
  private string _tenantId = default!;
  private bool _enableSoftDelete = false;
  private List<AccessPolicyEntryArgs> _accessPolicies = new List<AccessPolicyEntryArgs>();

  public KeyVaultBuilder() : base(ResourceType.KeyVault)
  {
  }

  public KeyVaultBuilder WithSKU(SkuName sku)
  {
    _sku = sku;
    return this;
  }

  public KeyVaultBuilder InTenant(string tenantId)
  {
    _tenantId = tenantId;
    return this;
  }

  public KeyVaultBuilder WithSoftDelete(bool value)
  {
    _enableSoftDelete = value;
    return this;
  }

  public KeyVaultBuilder WithPermissiveAccessPolicy(string objectId)
  {
    _accessPolicies.Add(new AccessPolicyEntryArgs()
    {
      TenantId = _tenantId,
      ObjectId = objectId, // The Object ID of a user, group, or service principal
      Permissions = new PermissionsArgs()
      {
        Secrets =
        {
          SecretPermissions.Get,
          SecretPermissions.List,
          SecretPermissions.Set,
          SecretPermissions.Delete
        }
      }
    });

    return this;
  }

  public override (ResourceGroup ResourceGroup, Vault Resource) Build()
  {
    var resourceGroup = GetResourceGroup();

    var managementLockByScope = new ManagementLockByScope("managementLockByScope", new()
    {
      Level = "CanNotDelete", // CanNotDelete | ReadOnly
      LockName = "lock",
      Scope = resourceGroup.Id,
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    var vault = new Vault(Name, new VaultArgs()
    {
      VaultName = Name,
      ResourceGroupName = resourceGroup.Name,
      Properties = new VaultPropertiesArgs()
      {
        TenantId = _tenantId,
        Sku = new SkuArgs()
        {
          Name = _sku,
          Family = "A"
        },
        EnableSoftDelete = _enableSoftDelete,
        EnabledForTemplateDeployment = true,
        EnabledForDeployment = true,
        NetworkAcls = new NetworkRuleSetArgs()
        {
        },
        AccessPolicies = new List<AccessPolicyEntryArgs>()
      }
    }, new CustomResourceOptions()
    {
      Protect = Protect
    });

    return (resourceGroup, vault);
  }
}
