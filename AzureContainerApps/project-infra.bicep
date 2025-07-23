param location string = resourceGroup().location
param projectName string
param stage string
param tags object
param managedIdName string
param registryName string
param environmentName string

module containerAppModule './container-app.bicep' = {
  name: '${deployment().name}-postgres'
  params: {
    location: location
    projectName: projectName
    stage: stage
    tags: tags
    azureContainerRegistry: registryName
    containerAppEnvName: environmentName
    managedIdName: managedIdName
  }
}
