@description('Name of the Resource Group')
param rgName string = 'DemoResourceGroup'

@description('Name of the storage account')
param storageName string = 'stg${uniqueString(rgName)}'

@description('Name of the container')
param containerName string = 'csv-upload'

// Note referencing of existing resource.
// See also https://github.com/Azure/bicep/blob/main/docs/spec/resources.md#referencing-existing-resources
resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: storageName
}

resource csvUploadContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: '${csvStorage.name}/default/${containerName}'
  properties: {
    publicAccess: 'None'
  }
}

output resourceID string = csvUploadContainer.id
