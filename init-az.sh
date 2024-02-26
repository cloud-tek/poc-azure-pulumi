az login
az account set --subscription <redacted>

pulumi config set azure-native:location polandcentral
pulumi config set PoC.Deployment.KeyVault:azure.tenantId <redacted>