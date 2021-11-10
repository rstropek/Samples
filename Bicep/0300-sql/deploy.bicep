@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'helloazuresql'

param location string = resourceGroup().location

param serverName string = 'sql-${uniqueString(baseName)}'
param sqlDBName string = 'sqldb-${uniqueString(baseName)}'

param csvUploadContainerName string = 'csv-upload'

@description('Indicates whether it should be possible to access this SQL Server over the public internet')
param allowInternetAccess bool = true

@description('Object ID of the AAD group that should become DB administrators')
param aadAdminTeamId string

param administratorLogin string = 'demoadmin'
@secure()
param administratorLoginPassword string

var insightsName = 'insights-${uniqueString(baseName)}'
var appServiceName = 'app-${uniqueString(baseName)}'
var testAppName = 'web-test-${uniqueString(baseName)}'
var storageName = 'st${uniqueString(baseName)}'

var roleIds = {
  storageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  storageBlobDataReader: '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1'
}

resource server 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: serverName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    minimalTlsVersion: '1.2'
    publicNetworkAccess: allowInternetAccess ? 'Enabled' : 'Disabled'
  }
  resource aadAdmin 'administrators@2020-11-01-preview' = {
    name: 'ActiveDirectory'
    properties: {
      administratorType: 'ActiveDirectory'
      login: 'Applications Team - Database Administrator'
      sid: aadAdminTeamId
      tenantId: subscription().tenantId
    }
  }
  resource fwAzureApps 'firewallRules@2020-11-01-preview' = {
    name: '${serverName}-fw-azureApps'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
  resource fwInternetAccess 'firewallRules@2020-11-01-preview' = if (allowInternetAccess) {
    name: '${serverName}-fw-internet'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '255.255.255.255'
    }
  }
}

resource sqlDB 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${server.name}/${sqlDBName}'
  location: location
  sku: {
    name: 'S1'
  }
  properties: {
  }
}

// For details about application insights see https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview
resource insights 'Microsoft.Insights/components@2015-05-01' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}

resource csvUploadContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: '${csvStorage.name}/default/${csvUploadContainerName}'
  properties: {
    publicAccess: 'None'
  }
}

resource storageAdminGroupAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, aadAdminTeamId)
  scope: csvStorage
  properties: {
    principalId: aadAdminTeamId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataContributor)
      // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
      // Note also bug/limitation https://github.com/Azure/bicep/issues/2031#issuecomment-816743989
  }
}

resource hosting 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServiceName
  location: location
  sku: {
    name: 'P1V2'          // See also https://azure.microsoft.com/en-us/pricing/details/app-service/windows/
    capacity: 1
  }
}

resource testApp 'Microsoft.Web/sites@2020-12-01' = {
  name: testAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.properties.InstrumentationKey
        }
        {
          name: 'ConnectionStrings:DefaultConnection'
          value: 'Server=${server.name}${environment().suffixes.sqlServerHostname}; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
        }
        {
          name: 'Storage:AccountName'
          value: '${csvStorage.name}'
        }
        {
          name: 'Storage:Container'
          value: csvUploadContainerName
        }
      ]
    }
  }
}

resource testStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, testApp.id)
  scope: csvStorage
  properties: {
    principalId: testApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataReader)
      // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
      // Note also bug/limitation https://github.com/Azure/bicep/issues/2031#issuecomment-816743989
  }
}

