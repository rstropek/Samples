// ====================================================================================================================
// PARAMETERS
//
@description('Name of the Resource Group')
param rgName string = 'Techorama2021-rehearsal'

@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'tor21r'

@description('Indicates whether it should be possible to access backend services over the public internet')
param allowInternetAccess bool = true

@description('Object ID of the AAD group that should become SQL DB administrators')
param aadAdminTeamId string

@description('Login name for SQL Server admin account')
param administratorLogin string = 'toradmin'

@description('Password for the SQL Server admin account')
@secure()
param administratorLoginPassword string

@description('Size of demo VM')
param VmSize string = 'Standard_DS2_v2'

@description('Login name for VM admin account')
param vmAdminUsername string = 'toradmin'

@description('Password for the VM admin account')
@secure()
param vmAdminPassword string

// ====================================================================================================================
// VARIABLES
//
// IP ranges
var vnetAddressPrefix = '10.0.0.0/16'
var subnetDefaultAddressPrefix = '10.0.1.0/24'

// Important naming constants
var uploadContainer = 'csv-upload'
var processedContainer = 'csv-processed'

// Helper variables for resource names
var appServiceblobName = 'stas${uniqueString(baseName)}'
var blobName = 'st${uniqueString(baseName)}'
var serverName = 'sql-${uniqueString(baseName)}'
var sqlDBName = 'sqldb-${uniqueString(baseName)}'
var vnetName = 'vnet-${baseName}-${uniqueString(baseName)}'
var vmSubnet = 'subnet-${baseName}-vm'
var vmName = 'vm-${baseName}-${uniqueString(baseName)}'
var functionName = '${baseName}-live-demo'

// See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
// Note also bug/limitation https://github.com/Azure/bicep/issues/2031#issuecomment-816743989
var roleIds = {
  storageBlobDataContributor: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

// ====================================================================================================================
// VNET
//
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: vmSubnet
        properties: {
          addressPrefix: subnetDefaultAddressPrefix
        }
      }
    ]
  }
}

// ====================================================================================================================
// APPLICATION INSIGHTS
//
resource insights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: 'insights-${uniqueString(baseName)}'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    DisableIpMasking: false // For demo purposes only. Consider GDPR in production scenarios.
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: allowInternetAccess ? 'Enabled' : 'Disabled'
  }
}

// ====================================================================================================================
// STORAGE
//
resource appServiceStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: appServiceblobName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
    tier: 'Premium'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}

resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: blobName
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
    tier: 'Premium'
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
  name: concat(csvStorage.name, '/default/', uploadContainer)
  properties: {
    publicAccess: 'None'
  }
}

resource csvProcessedContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-02-01' = {
  name: concat(csvStorage.name, '/default/', processedContainer)
  properties: {
    publicAccess: 'None'
  }
}

// Allow AAD administrators to contribute to storage
resource adminStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, aadAdminTeamId)
  scope: csvStorage
  properties: {
    principalId: aadAdminTeamId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataContributor)
  }
}

// ====================================================================================================================
// SQL
//
resource server 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: serverName
  location: resourceGroup().location
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
    name: concat(serverName, '-fw-azureApps')
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
  resource fwInternetAccess 'firewallRules@2020-11-01-preview' = if (allowInternetAccess) {
    name: concat(serverName, '-fw-internet')
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '255.255.255.255'
    }
  }
  resource sqlDB 'databases@2020-11-01-preview' = {
    name: sqlDBName
    location: resourceGroup().location
    sku: {
      name: 'S0'
      tier: 'Standard'
    }
  }
}

// ====================================================================================================================
// FUNCTION
//
resource hosting 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: 'app-${uniqueString(baseName)}'
  location: resourceGroup().location
  sku: {
    name: 'EP1'
  }
}

resource function 'Microsoft.Web/sites@2020-12-01' = {
  name: functionName
  location: resourceGroup().location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      ftpsState: 'Disabled'
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: insights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appServiceStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(appServiceStorage.id, appServiceStorage.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${appServiceStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(appServiceStorage.id, appServiceStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'StorageConnection'
          value: 'https://${csvStorage.name}.blob.core.windows.net/csv-processed'
        }
        {
          name: 'SqlConnection'
          value: 'Server=${server.name}.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
        }
        {
          name: 'WEBSITE_DNS_SERVER'
          value: '168.63.129.16'
        }
        {
          name: 'WEBSITE_VNET_ROUTE_ALL'
          value: '1'
        }
      ]
    }
  }
}

// Allow Azure Function to access storage
resource functionStorageAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(csvStorage.id, function.id)
  scope: csvStorage
  properties: {
    principalId: function.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.storageBlobDataContributor)
  }
}

// ====================================================================================================================
// VIRTUAL MACHINE
// This is for demo purposes only! Not needed in production.
//
resource publicIpAddressName 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: 'ip-${baseName}-${uniqueString(baseName)}'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: toLower(vmName)
    }
  }
}

resource networkInterfaceName 'Microsoft.Network/networkInterfaces@2020-06-01' = {
  name: 'nic-${baseName}-${uniqueString(baseName)}'
  location: resourceGroup().location
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
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, vmSubnet)
          }
        }
      }
    ]
  }
}

resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: vmName
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: VmSize
    }
    osProfile: {
      computerName: vmName
      adminUsername: vmAdminUsername
      adminPassword: vmAdminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'Canonical'
        offer: 'UbuntuServer'
        sku: '18.04-LTS'
        version: 'latest'
      }
      osDisk: {
        name: '${vmName}OsDisk'
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

output storageConnection string = 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
output sqlConnection string = 'Server=${server.name}.database.windows.net; Authentication=Active Directory MSI; Initial Catalog=${sqlDBName};'
output functionDomainVerification string = function.properties.customDomainVerificationId
