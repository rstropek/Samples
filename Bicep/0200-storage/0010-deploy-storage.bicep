@description('Name of the Resource Group')
param rgName string = 'DemoResourceGroup'

@description('Indicates whether read-only-lock should be created')
param placeLock bool = true

var storageName = 'stg${uniqueString(rgName)}'

resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
  location: resourceGroup().location
  tags: {
    Dept: 'IT'
    Environment: 'Test'
  }
  sku: {
    name: 'Standard_LRS'  // See also https://docs.microsoft.com/en-us/rest/api/storagerp/srp_sku_types
  }
  kind: 'StorageV2'       // See also https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview#types-of-storage-accounts
  properties: {
    accessTier: 'Hot'     // See also https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-storage-tiers
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}

// For locks see also https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/lock-resources
// Note conditional deployment of resource (see also https://github.com/Azure/bicep/blob/main/docs/spec/resources.md#conditions)
resource csvStorageLock 'Microsoft.Authorization/locks@2016-09-01' = if (placeLock) {
  name: '${storageName}-lock'
  scope: csvStorage
  properties: {
    level: 'CanNotDelete'
    notes: 'Production storage account, must not be deleted'
  }
}

output resourceID string = csvStorage.id
output storageName string = csvStorage.name
