@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'hellofunctions'

param location string = resourceGroup().location
param uploadContainer string = 'csv-upload'

var appServiceName = 'app-${uniqueString(baseName)}'
var functionNameNet3 = 'func-${uniqueString(baseName)}-netcore3'
var functionNameNet6 = 'func-${uniqueString(baseName)}-net6'
var storageName = 'st${uniqueString(baseName)}'
var serviceBusName = 'sb-${uniqueString(baseName)}'
var successTopicName = 'importsuccess'
var errorTopicName = 'importerror'
var successSubscriptionName = 'successlog'
var errorSubscriptionName = 'errorlog'
var serviceBusAuthorizationName = 'sendandlisten'

resource csvStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: storageName
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
  name: '${csvStorage.name}/default/${uploadContainer}'
  properties: {
    publicAccess: 'None'
  }
}

resource hosting 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServiceName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource sbName 'Microsoft.ServiceBus/namespaces@2021-01-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Standard'
  }
  resource successTopic 'topics@2018-01-01-preview' = {
    name: successTopicName
    properties: {
      autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
      defaultMessageTimeToLive: 'PT5M'
    }
    resource successSubscription 'subscriptions@2018-01-01-preview' = {
      name: successSubscriptionName
      properties: {
        autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
        defaultMessageTimeToLive: 'PT5M'
      }
    }
  }
  resource errorTopic 'topics@2018-01-01-preview' = {
    name: errorTopicName
    properties: {
      autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
      defaultMessageTimeToLive: 'PT5M'
    }
    resource errorSubscription 'subscriptions@2018-01-01-preview' = {
      name: errorSubscriptionName
      properties: {
        autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
        defaultMessageTimeToLive: 'PT5M'
      }
    }
  }
  resource serviceBusAuthorization 'AuthorizationRules@2017-04-01' = {
    name: serviceBusAuthorizationName
    properties: {
      rights: [
        'Send'
        'Listen'
      ]
    }
  }
}

resource netCore3Func 'Microsoft.Web/sites@2020-12-01' = {
  name: functionNameNet3
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v3.1'
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
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
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'ServiceBusConnection'
          value: 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
        }
        {
          name: 'StorageConnection'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
      ]
    }
  }
}

resource netCore6Func 'Microsoft.Web/sites@2020-12-01' = {
  name: functionNameNet6
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hosting.id
    siteConfig: {
      netFrameworkVersion: 'v5.0'
      cors: {
        allowedOrigins: [
          '*'
        ]
      }
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'ServiceBusConnection'
          value: 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
        }
        {
          name: 'StorageConnection'
          value: 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
        }
        {
          name: 'netFrameworkVersion'
          value: 'v5.0'
        }
      ]
    }
  }
}

output storageConnection string = 'DefaultEndpointsProtocol=https;AccountName=${csvStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(csvStorage.id, csvStorage.apiVersion).keys[0].value}'
output serviceBusConnection string = 'Endpoint=sb://${serviceBusName}.servicebus.windows.net/;SharedAccessKeyName=${serviceBusAuthorizationName};SharedAccessKey=${listKeys(sbName::serviceBusAuthorization.id, '2017-04-01').primaryKey}'
