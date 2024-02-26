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

- 1 stack = 1 rgp, unable to share RGP across stacks

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
