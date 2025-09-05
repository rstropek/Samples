@description('Name of the project, uses oaiazure by default')
param projectName string = 'oaiazure'

@description('Deployment location, uses Sweden Central by default')
param location string = 'swedencentral'

param oaiPrincipals array = []

var tags = {
  Project: projectName
}

var abbrs = loadJsonContent('abbreviations.json')

var oaiName = '${abbrs.cognitiveServicesAccounts}${uniqueString(projectName)}'
var searchServiceName = '${abbrs.searchSearchServices}${uniqueString(projectName)}'

module networkModule './network.bicep' = {
  name: '${deployment().name}-networkDeploy'
  params: {
    location: location
    projectName: projectName
    tags: tags
  }
}

module oai './oai.bicep' = {
  name: '${deployment().name}-oaiDeploy'
  params: {
    location: location
    projectName: projectName
    tags: tags
    peSubnetId: networkModule.outputs.peSubnetId
    dnsZoneId: networkModule.outputs.privateDnsZoneId
    oaiName: oaiName
    principalIds: union([
      web.outputs.managedIdentityPrincipalId
    ], oaiPrincipals)
  }
}


module search './search.bicep' = {
  name: '${deployment().name}-searchDeploy'
  params: {
    location: location
    projectName: projectName
    tags: tags
    peSubnetId: networkModule.outputs.peSubnetId
    dnsZoneId: networkModule.outputs.privateDnsZoneSearchId
    searchServiceName: searchServiceName
    principalIds: union([
      web.outputs.managedIdentityPrincipalId
    ], oaiPrincipals)
  }
}

module web './app-service.bicep' = {
  name: '${deployment().name}-appServiceDeploy'
  params: {
    location: location
    projectName: projectName
    tags: tags
    subnetId: networkModule.outputs.webSubnetId
    accountName: oaiName
    searchServiceName: searchServiceName
  }
}

output accountName string = oai.outputs.accountName
