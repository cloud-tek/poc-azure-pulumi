## Limitations

[link](https://www.pulumi.com/blog/full-coverage-of-azure-resources-with-azure-native/)

> Unfortunately, some Azure capabilities aren’t yet available via the Azure Resource Manager APIs used by the native Azure provider. To ensure that you can still use some of the missing capabilities, we’ve introduced several resources that go beyond what’s currently possible with ARM (and ARM templates):
>
> - Exposing Azure Storage as a static website.
> - Uploading files and directories as Azure Storage Blobs.
> - Managing Azure KeyVault secrets and keys.
>
> By combining full support for the ARM APIs with these additional extensions, Pulumi’s native Azure provider provides the broadest support for managing Azure resources with infrastructure-as-code.*

## Conclusions

### 1 stack = 1 rgp, unable to share RGP across stacks

```
Updating (dev):
     Type                                     Name                        Status
 +   pulumi:pulumi:Stack                      PoC.Deployment.Storage-dev  **creating failed (0.11s)**
 +   └─ azure-native:resources:ResourceGroup  poc-pulumi-rgp2             **creating failed**


Diagnostics:
  azure-native:resources:ResourceGroup (poc-pulumi-rgp2):
    error: cannot create already existing subresource '/subscriptions/6c0fc852-0fc4-4f70-8e70-272726886c14/resourcegroups/poc-pulumi-rgp2'

  pulumi:pulumi:Stack (PoC.Deployment.Storage-dev):
    error: update failed
```

### Stack Ids

- Pulumi stack id consists of: `{organization}/{namespace}/{stack-name}`
- Deployed Pulumi Stacks can be listed using `pulumi ls`

```
pulumi stack ls --all
NAME                                                 LAST UPDATE   RESOURCE COUNT
organization/PoC.Deployment.KeyVault.Tests/ayigieuv  7 hours ago   0
organization/PoC.Deployment.KeyVault.Tests/eemoycch  7 hours ago   1
organization/PoC.Deployment.KeyVault.Tests/fdthriur  n/a           n/a
organization/PoC.Deployment.KeyVault.Tests/ierjdqqo  n/a           n/a
organization/PoC.Deployment.KeyVault.Tests/kcduhbwb  n/a           n/a
organization/PoC.Deployment.KeyVault.Tests/qljaatrl  7 hours ago   0
organization/PoC.Deployment.KeyVault.Tests/qnwbwbxe  6 hours ago   0
organization/PoC.Deployment.KeyVault.Tests/sxkyebcq  n/a           n/a
organization/PoC.Deployment.KeyVault.Tests/zjgffzrp  7 hours ago   1
organization/PoC.Deployment.KeyVault/dev             23 hours ago  5
organization/PoC.Deployment.Network/dev              23 hours ago  10
organization/PoC.Deployment.Storage/dev              23 hours ago  9
```

### Configurability

- Native Pulumi configuration originates from the `Pulumi.*.yaml` files
- Native Pulumi secrets originate from the `Pulumi.*.yaml` files and are kept under source control (encrypted)
- It is possible to use `IConfiguration` when using dotnet

### State & StackReferences

At the moment I don't know if it's possible to reference resources from other state(s).

The state file is a container for all deployed stacks.
They can be listed using the CLI

```bash
pulumi stack ls --all
NAME                                      LAST UPDATE    RESOURCE COUNT
organization/PoC.Deployment.KeyVault/dev  5 minutes ago  5
organization/PoC.Deployment.Network/dev   5 minutes ago  10
organization/PoC.Deployment.Storage/dev   4 minutes ago  9
```

### Orchestrating stack deployments

Complex deployment of multiple stacks (like a full-env deployment) can be achieved in 2 ways:
- [Pulumi.AutoDeploy](https://www.pulumi.com/registry/packages/auto-deploy/)
- [NUKE](https://www.techwatching.dev/posts/when-pulumi-met-nuke?fbclid=IwAR2It7Nn-WJBAuxiDRgpKJ5XvTkMp5sHZofpS4p9NSCgMMA5urQUJy2OOo0)

This repo contains a NUKE sample deploying some infrastructure

### Using builders

Using raw pulumi DSL will not work on large scale, but still permits raw usage if one wishes to.

### returning (ResourceGroup, TResource)

When referencing Azure resources (like adding a secret to a keyvault), both the resource group and a resource are needed. A TResource does not expose a ResourceGroup property, even though it is required at creation time. Therefore, the builder needs to return a tuple containing both the resource group and the resource.

*See: PoC.Deployment.Storage**

```csharp
var kvRef = new StackReference($"organization/PoC.Deployment.KeyVault/{Pulumi.Deployment.Instance.StackName}");
    var vaultRef = (
      ResourceGroup: kvRef.RequireOutput("ResourceGroupName").Apply(x => x.ToString())!,
      Resource: kvRef.RequireOutput("Name").Apply(x => x.ToString())!);


var storage = new StorageAccountBuilder()
  .In(rgp)
  .WithSKU(SkuName.Standard_LRS)
  .DisableProtection()
  .Build();

  storage
    .AddPrimaryKeyToKeyVault(vaultRef!, "data-PrimaryKey")
    .AddSecondaryKeyToKeyVault(vaultRef!, "data-SecondaryKey");
```
