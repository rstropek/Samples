@description('Name of the project, uses oaiazure by default')
param projectName string = 'oaiazure'

@description('Deployment location, uses Sweden Central by default')
param location string = 'swedencentral'

param oaiPrincipals array = []

var tags = {
  Project: projectName
}

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
    principalIds: union([
      web.outputs.managedIdentityPrincipalId
    ], oaiPrincipals)
  }
  dependsOn: [
    networkModule
  ]
}

module web './app-service.bicep' = {
  name: '${deployment().name}-appServiceDeploy'
  params: {
    location: location
    projectName: projectName
    tags: tags
    subnetId: networkModule.outputs.webSubnetId
  }
  dependsOn: [
    networkModule
  ]
}

output accountName string = oai.outputs.accountName
