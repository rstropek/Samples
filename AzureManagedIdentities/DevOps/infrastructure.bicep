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

param vmSize string = 'Standard_DS2_v2'

@description('Indicates whether accessing function from public internet should be allowed')
param funcPublicAccess string = 'Enabled'

@description('Login name for VM admin account (VM is for demo purposes only, not needed in real life)')
param vmAdminUsername string = 'rainer'

@description('Password for the VM admin account (VM is for demo purposes only, not needed in real life)')
@secure()
param vmAdminPassword string = 'P@ssw0rd!1234'

@description('Optional ID of admin user, this user will have access to storage services with customer data; for dev/test stages only')
param adminPrincipalId string = ''

@description('Optional name of admin user, this user will have access to storage services with customer data; for dev/test stages only')
param adminPrincipalName string = ''

@description('Optional name of the container to deploy to web app')
param containerName string = 'nginx:alpine'

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
  containerRegistry: 'acr${uniqueString(baseName)}'
  vnet: 'vnet-${uniqueString(baseName)}'
  vmSubnet: 'subnet-${uniqueString('${baseName}-vm')}'
  peSubnet: 'subnet-${uniqueString('${baseName}-pe')}'
  webSubnet: 'subnet-${uniqueString('${baseName}-web')}'
  peDataStorage: 'pe-st-${uniqueString('${baseName}')}'
  peSql: 'pe-sql-${uniqueString('${baseName}')}'
  peFunc: 'pe-func-${uniqueString('${baseName}')}'
  peKeyVault: 'pe-keyvault-${uniqueString('${baseName}')}'
  blobPeDnsZoneName: 'privatelink.blob.${environment().suffixes.storage}'
  keyVaultPeDnsZoneName: 'privatelink.vaultcore.azure.net'
  sqlPeDnsZoneName: 'privatelink${environment().suffixes.sqlServerHostname}'
  funcPeDnsZoneName: 'privatelink.azurewebsites.net'
  publicIpVm: 'pip-${uniqueString('${baseName}-vm')}'
  vm: 'vm-${uniqueString('${baseName}')}'
  nicVm: 'nic-${uniqueString('${baseName}-vm')}'
  appInsights: 'appi-${uniqueString('${baseName}')}'
  logAnalytics: 'log-${uniqueString('${baseName}')}'
  keyVaultDiagSettings: 'kv-settings-${uniqueString(baseName)}'
}
var ips = {
  vnetAddressPrefix: '10.0.0.0/16'
  subnetDefaultAddressPrefix: '10.0.1.0/24'
  subnetPrivateEndpointAddressPrefix: '10.0.2.0/24'
  subnetWebAppsAddressPrefix: '10.0.3.0/24'
}
var tags = {
  Project: projectName
}
var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  KeyVaultSecretsUser: '4633458b-17de-408a-b874-0445c86b69e6'
  KeyVaultSecretsOfficer: 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  StorageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  AcrPull: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  AcrPush: '8311e382-0749-4cb8-b61a-304f252e45ec'
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: names.vnet
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        ips.vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: names.vmSubnet
        properties: {
          addressPrefix: ips.subnetDefaultAddressPrefix
        }
      }
      {
        name: names.peSubnet
        properties: {
          addressPrefix: ips.subnetPrivateEndpointAddressPrefix

          // Disabling private endpoint NSGs is required. Read more at
          // https://docs.microsoft.com/en-us/azure/private-link/disable-private-endpoint-network-policy
          privateEndpointNetworkPolicies: 'Disabled'
        }
      }
      {
        name: names.webSubnet
        properties: {
          addressPrefix: ips.subnetWebAppsAddressPrefix
          delegations: [
            {
              name: 'webapp-subnet-delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }            
          ]
        }
      }
    ]
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: names.logAnalytics
  location: location
  tags: tags
  properties: {
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    retentionInDays: 30
    features: {
      disableLocalAuth: false
      enableDataExport: false
    }
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: names.appInsights
  location: location
  tags: union(tags, {
      'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${names.functionApp}': 'Resource'
      'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${names.webApp}': 'Resource'
    })
  kind: 'web'
  properties: {
    Application_Type: 'web'
    DisableIpMasking: false
    WorkspaceResourceId: logAnalytics.id
  }
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
    publicNetworkAccess: 'Disabled'
  }
}

resource blobPe 'Microsoft.Network/privateEndpoints@2022-05-01' = {
  name: names.peDataStorage
  location: location
  tags: tags
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.peSubnet)
    }
    privateLinkServiceConnections: [
      {
        name: names.peDataStorage
        properties: {
          privateLinkServiceId: dataStorage.id
          groupIds: [
            'blob'
          ]
        }
      }
    ]
  }
  
  // Integrate private endpoint for storage with private DNS zone
  resource blobPeDnsGroup 'privateDnsZoneGroups@2022-05-01' = {
    name: 'storagepednsgroupname'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'config1'
          properties: {
            privateDnsZoneId: privateDnsZoneBlob.id
          }
        }
      ]
    }
  }

  dependsOn: [
    vnet
  ]
}

resource privateDnsZoneBlob 'Microsoft.Network/privateDnsZones@2020-01-01' = {
  name: names.blobPeDnsZoneName
  location: 'global'
  tags: tags
  properties: { }

  resource privateDnsZoneBlob_privateDnsZoneName_link 'virtualNetworkLinks@2020-01-01' = {
    name: names.blobPeDnsZoneName
    location: 'global'
    properties: {
      registrationEnabled: false
      virtualNetwork: {
        id: vnet.id
      }
    }
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
      publicNetworkAccess: funcPublicAccess
    }
  }

  resource settings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey
      AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'
      FUNCTIONS_EXTENSION_VERSION: '~4'
      FUNCTIONS_WORKER_RUNTIME: 'dotnet'
      WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${functionStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorage.id, functionStorage.apiVersion).keys[0].value}'
      AZURE_FUNCTIONS_ENVIRONMENT: 'Development'
      AzureWebJobsDisableHomepage: 'true'
      WEBSITE_CONTENTSHARE: names.functionApp
      WEBSITE_RUN_FROM_PACKAGE: '1'
      AadTenantId: subscription().tenantId
      Audience: 'api://backend-api.rainertimecockpit.onmicrosoft.com'
      WEBSITE_DNS_SERVER: '168.63.129.16'
      WEBSITE_VNET_ROUTE_ALL: '1'
    }
  }

  resource functionVnet 'networkConfig@2020-10-01' = {
    name: 'virtualNetwork'
    properties: {
      subnetResourceId: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.webSubnet)
      swiftSupported: true
    }
  }
}

resource privateDnsZoneFunc 'Microsoft.Network/privateDnsZones@2020-01-01' = {
  name: names.funcPeDnsZoneName
  location: 'global'
  tags: tags
  properties: { }

  resource privateDnsZoneFunc_privateDnsZoneName_link 'virtualNetworkLinks@2020-01-01' = {
    name: names.funcPeDnsZoneName
    location: 'global'
    properties: {
      registrationEnabled: false
      virtualNetwork: {
        id: vnet.id
      }
    }
  }
}

resource funcPe 'Microsoft.Network/privateEndpoints@2022-05-01' = {
  name: names.peFunc
  location: location
  tags: tags
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.peSubnet)
    }
    privateLinkServiceConnections: [
      {
        name: names.peFunc
        properties: {
          privateLinkServiceId: functionApp.id
          groupIds: [
            'sites'
          ]
        }
      }
    ]
  }
  
  // Integrate private endpoint for storage with private DNS zone
  resource funcPeDnsGroup 'privateDnsZoneGroups@2022-05-01' = {
    name: 'funcpednsgroupname'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'config1'
          properties: {
            privateDnsZoneId: privateDnsZoneFunc.id
          }
        }
      ]
    }
  }

  dependsOn: [
    vnet
  ]
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

      acrUseManagedIdentityCreds: true
      linuxFxVersion: 'DOCKER|${containerName}'
    }
  }

  resource settings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey
      WEBSITE_RUN_FROM_PACKAGE: '1'
      AccountNames__Storage: 'stuu3g75ep5thny'
      AccountNames__KeyVault: 'kv-sbwa7dm5lugos'
      AccountNames__DbServer: 'sql-sbwa7dm5lugos'
      AccountNames__Database: 'sqldb-sbwa7dm5lugos'
      AccountNames__Backend: 'func-sbwa7dm5lugos.azurewebsites.net'
      AccountNames__Audience: 'api://backend-api.rainertimecockpit.onmicrosoft.com'
      WEBSITE_DNS_SERVER: '168.63.129.16'
      WEBSITE_VNET_ROUTE_ALL: '1'
    }
  }

  resource webAppVnet 'networkConfig@2020-10-01' = {
    name: 'virtualNetwork'
    properties: {
      subnetResourceId: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.webSubnet)
      swiftSupported: true
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
    publicNetworkAccess: 'Disabled'
  }
}

resource keyvaultDiagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: names.keyVaultDiagSettings
  scope: keyvault
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        categoryGroup: 'audit'
        enabled: true
      }
      {
        categoryGroup: 'allLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

resource keyVaultPe 'Microsoft.Network/privateEndpoints@2022-05-01' = {
  name: names.peKeyVault
  location: location
  tags: tags
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.peSubnet)
    }
    privateLinkServiceConnections: [
      {
        name: names.peKeyVault
        properties: {
          privateLinkServiceId: keyvault.id
          groupIds: [
            'vault'
          ]
        }
      }
    ]
  }

  resource keyVaultPeDnsGroup 'privateDnsZoneGroups@2022-05-01' = {
    name: 'keyvaultpednsgroupname'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'config1'
          properties: {
            privateDnsZoneId: privateDnsZoneKeyVault.id
          }
        }
      ]
    }
  }

  dependsOn: [
    vnet
  ]
}

resource privateDnsZoneKeyVault 'Microsoft.Network/privateDnsZones@2020-01-01' = {
  name: names.keyVaultPeDnsZoneName
  location: 'global'
  tags: tags
  properties: { }

  resource privateDnsZoneKeyVault_privateDnsZoneName_link 'virtualNetworkLinks@2020-01-01' = {
    name: names.keyVaultPeDnsZoneName
    location: 'global'
    properties: {
      registrationEnabled: false
      virtualNetwork: {
        id: vnet.id
      }
    }
  }
}

resource functionAppKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(keyvault.id, functionApp.id)
  scope: keyvault
  properties: {
    principalId: functionApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsOfficer)
  }
}

resource webAppKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(keyvault.id, webApp.id)
  scope: keyvault
  properties: {
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsOfficer)
  }
}

resource adminKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (adminPrincipalId != '') {
  name: guid(keyvault.id, adminPrincipalId)
  scope: keyvault
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsOfficer)
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

resource sqlPe 'Microsoft.Network/privateEndpoints@2022-05-01' = {
  name: names.peSql
  location: location
  tags: tags
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.peSubnet)
    }
    privateLinkServiceConnections: [
      {
        name: names.peSql
        properties: {
          privateLinkServiceId: sqlServer.id
          groupIds: [
            'sqlServer'
          ]
        }
      }
    ]
  }

  resource sqlPeDnsGroup 'privateDnsZoneGroups@2022-05-01' = {
    name: 'sqlpednsgroupname'
    properties: {
      privateDnsZoneConfigs: [
        {
          name: 'config1'
          properties: {
            privateDnsZoneId: privateDnsZoneSql.id
          }
        }
      ]
    }
  }

  dependsOn: [
    vnet
  ]
}

resource privateDnsZoneSql 'Microsoft.Network/privateDnsZones@2020-01-01' = {
  name: names.sqlPeDnsZoneName
  location: 'global'
  tags: tags
  properties: { }

  resource privateDnsZoneSql_privateDnsZoneName_link 'virtualNetworkLinks@2020-01-01' = {
    name: names.sqlPeDnsZoneName
    location: 'global'
    properties: {
      registrationEnabled: false
      virtualNetwork: {
        id: vnet.id
      }
    }
  }
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' = {
  name: names.containerRegistry
  location: location
  tags: tags
  sku: {
    name: 'Standard'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource webAppAcrAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(containerRegistry.id, webApp.id)
  scope: containerRegistry
  properties: {
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.AcrPull)
  }
}

resource adminAcrAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = if (adminPrincipalId != '') {
  name: guid(containerRegistry.id, adminPrincipalId)
  scope: containerRegistry
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.AcrPush)
  }
}

resource publicIpAddressName 'Microsoft.Network/publicIPAddresses@2022-05-01' = {
  name: names.publicIpVm
  tags: tags
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: toLower(names.vm)
    }
  }
}

resource networkInterfaceName 'Microsoft.Network/networkInterfaces@2020-06-01' = {
  name: names.nicVm
  location: location
  tags: tags
  properties: {
    ipConfigurations: [
      {
        name: 'ipConfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: publicIpAddressName.id
          }
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', names.vnet, names.vmSubnet)
          }
        }
      }
    ]
  }
  dependsOn: [
    vnet
  ]
}

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: names.vm
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: names.vm
      adminUsername: vmAdminUsername
      adminPassword: vmAdminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'canonical'
        offer: '0001-com-ubuntu-server-jammy'
        sku: '22_04-lts-gen2'
        version: 'latest'
      }
      osDisk: {
        name: '${names.vm}OsDisk'
        caching: 'ReadWrite'
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'Standard_LRS'
        }
        diskSizeGB: 128
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: networkInterfaceName.id
        }
      ]
    }
  }
}

resource vmKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(keyvault.id, vm.id)
  scope: keyvault
  properties: {
    principalId: vm.identity.principalId
    principalType: 'ServicePrincipal' // This is important for managed identities to prevent timeouts
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.KeyVaultSecretsOfficer)
  }
}

output funcName string = functionApp.name
output webAppName string = webApp.name
