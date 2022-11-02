@description('Name of the project')
param projectName string = 'azmgedid'

@description('Location of the resources, uses resource group\'s location by default')
param location string = resourceGroup().location

@allowed([
  'P1V3'
  'P1V2'
  'S1'
])
param appServicePlanSku string = 'P1V3'

@description('Optional ID of admin user, this user will have access to storage services with customer data; for dev/test stages only')
param adminPrincipalId string = ''

@description('Optional name of admin user, this user will have access to storage services with customer data; for dev/test stages only')
param adminPrincipalName string = ''

var baseName = projectName
var names = {
  dataStorage: 'st${uniqueString('${baseName}-data')}'
  functionStorage: 'st${uniqueString('${baseName}-functionstorage')}'
  functionIdentity: 'id-${uniqueString(baseName)}'
  appServicePlan: 'plan-${uniqueString(baseName)}'
  functionApp: 'func-${uniqueString(baseName)}'
  webApp: 'app-${uniqueString(baseName)}'
  keyVault: 'kv-${uniqueString(baseName)}'
  sqlServer: 'sql-${uniqueString(baseName)}'
  database: 'sqldb-${uniqueString(baseName)}'
}
var tags = {
  Project: projectName
}
var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  KeyVaultSecretsUser: '4633458b-17de-408a-b874-0445c86b69e6'
  StorageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

resource dataStorage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: names.dataStorage
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Cool'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
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

resource functionStorage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: names.functionStorage
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Cool'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true // Just config data, no customer data
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource adminFunctionStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (adminPrincipalId != '') {
  name: guid(functionStorage.id, adminPrincipalId)
  scope: functionStorage
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.StorageBlobDataContributor)
  }
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

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: names.functionApp
  location: location
  tags: tags
  kind: 'functionapp'
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
      functionAppScaleLimit: 5
      webSocketsEnabled: false
    }
  }

  resource settings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'
      FUNCTIONS_EXTENSION_VERSION: '~4'
      FUNCTIONS_WORKER_RUNTIME: 'dotnet'
      WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'
      AZURE_FUNCTIONS_ENVIRONMENT: 'Development'
      AzureWebJobsDisableHomepage: 'true'
      WEBSITE_CONTENTSHARE: names.functionApp
      WEBSITE_RUN_FROM_PACKAGE: '1'
    }
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
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
      webSocketsEnabled: false
    }
  }

  resource settings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      WEBSITE_RUN_FROM_PACKAGE: '1'
    }
  }
}

resource functionDataStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(dataStorage.id, functionApp.id)
  scope: dataStorage
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.StorageBlobDataContributor)
  }
}

resource webAppDataStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(dataStorage.id, webApp.id)
  scope: dataStorage
  properties: {
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.StorageBlobDataContributor)
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: names.keyVault
  location: location
  tags: tags
  properties: {
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableRbacAuthorization: true // Important because vault contains secrets
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
  }
}

resource functionAppKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(keyvault.id, functionApp.id)
  scope: keyvault
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsUser)
  }
}

resource webAppKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(keyvault.id, webApp.id)
  scope: keyvault
  properties: {
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsUser)
  }
}

resource adminKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (adminPrincipalId != '') {
  name: guid(keyvault.id, adminPrincipalId)
  scope: keyvault
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsUser)
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: names.sqlServer
  location: location
  tags: tags
  properties: {
    version: '12.0'
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      principalType: 'User'
      tenantId: subscription().tenantId
      login: adminPrincipalName
      sid: adminPrincipalId
    }
    minimalTlsVersion: '1.2'
  }

  resource firewallRuleAzureInternal 'firewallRules@2022-05-01-preview' = {
    name: 'AzureInternal'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }

  resource firewallRule 'firewallRules@2022-05-01-preview' = {
    name: 'AllowAll'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '255.255.255.255'
    }
  }

  resource database 'databases@2022-05-01-preview' = {
    name: names.database
    location: location
    tags: tags
    sku: {
      name: 'GP_S_Gen5_1'
      tier: 'GeneralPurpose'
      family: 'Gen5'
      capacity: 1
    }
    properties: {
      collation: 'SQL_Latin1_General_CP1_CI_AS'
      catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
      zoneRedundant: false
      readScale: 'Disabled'
    }
  }
}

output funcName string = functionApp.name
output webAppName string = webApp.name
