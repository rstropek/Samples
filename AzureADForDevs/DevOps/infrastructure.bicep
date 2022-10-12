@description('Name of the project, uses azureadfordevs by default')
param projectName string = 'azureadfordevs'

@description('Location of the resources, uses resource group\'s location by default')
param location string = resourceGroup().location

@allowed([
  'dev'
  'test'
  'prod'
])
@description('Stage of the deployment, uses dev by default')
param stage string = 'dev'

@allowed([
  'P1V3'
  'P2V3'
  'P3V3'
])
param appServicePlanSku string = 'P1V3'

@description('Optional ID of admin user, this user will have access to storage services with customer data; for dev/test stages only')
param adminPrincipalId string = ''

var baseName = '${stage}-${projectName}'
var names = {
  appServicePlan: 'plan-${uniqueString('${baseName}')}'
  webApp: 'web-${uniqueString('${baseName}')}'
  dataStorage: 'st${uniqueString('${baseName}-data')}'
}
var tags = {
  Project: projectName
  Environment: stage
}
var tagsCustomerData = {
  DataClassification: 'Customer Data'
}
var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  StorageBlobDataReader: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
  StorageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: names.appServicePlan
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true
  }
  kind: 'linux'
}

resource dataStorage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: names.dataStorage
  location: location
  tags: union(tags, tagsCustomerData)
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
  
  resource dataContainer 'blobServices' = {
    name: 'default'
    properties: {
      isVersioningEnabled: true
    }

    resource dataContainer 'containers@2022-05-01' = {
      name: 'data'
      properties: {
        publicAccess: 'None'
      }
    }
  }
}

resource webApp 'Microsoft.Web/sites@2021-03-01' = {
  name: names.webApp
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    httpsOnly: true
    serverFarmId: appServicePlan.id
    clientAffinityEnabled: true
    siteConfig: {
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      http20Enabled: true
    }
  }

  resource settings 'config@2021-03-01' = {
    name: 'appsettings'
    properties: {
    }
  }
}

resource webAppDataStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(dataStorage.id, webApp.id)
  scope: dataStorage
  properties: {
    principalId: webApp.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.StorageBlobDataReader)
  }
}

resource adminDataStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (adminPrincipalId != '') {
  name: guid(dataStorage.id, adminPrincipalId)
  scope: dataStorage
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.StorageBlobDataContributor)
  }
}
