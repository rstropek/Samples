param location string = resourceGroup().location

param projectName string

param tags object

param subnetId string

param accountName string
param searchServiceName string

var abbrs = loadJsonContent('abbreviations.json')

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: '${abbrs.webServerFarms}${uniqueString(projectName)}'
  location: location
  tags: tags
  sku: {
    name: 'P0v3'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: '${abbrs.webSitesAppService}${uniqueString(projectName)}'
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|rstropek/rag-webapp'
      alwaysOn: true
      cors: {
        allowedOrigins: ['*']
      }
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
    }
    virtualNetworkSubnetId: subnetId
    vnetRouteAllEnabled: true
  }

  resource settings 'config@2024-04-01' = {
    name: 'appsettings'
    properties: {
      AZURE_ENDPOINT: 'https://${accountName}.openai.azure.com/'
      AZURE_DEPLOYMENT: 'gpt-4.1'
      SEARCH_ENDPOINT: 'https://${searchServiceName}.search.windows.net'
      PORT: '3000'
      DOCKER_REGISTRY_SERVER_URL: 'https://index.docker.io'
    }
  }
}

output managedIdentityPrincipalId string = webApp.identity.principalId
