param location string = resourceGroup().location
param projectName string
param stage string
param tags object
param managedIdName string
param azureContainerRegistry string
param containerAppEnvName string
param containerImageName string = 'demo-app'

var abbrs = loadJsonContent('abbreviations.json')

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: managedIdName
}

resource environment 'Microsoft.App/managedEnvironments@2025-01-01' existing = {
  name: containerAppEnvName
}

// Create container app
resource app 'Microsoft.App/containerApps@2025-01-01' = {
  name: '${abbrs.appContainerApps}${uniqueString(projectName, stage)}'
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    environmentId: environment.id
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 3000
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: [
        {
          server: '${azureContainerRegistry}.azurecr.io'
          identity: identity.id
        }
      ]
    }
    template: {
      containers: [
        {
          image: '${azureContainerRegistry}.azurecr.io/${containerImageName}:latest'
          name: containerImageName
          resources: {
            cpu: 1
            memory: '2Gi'
          }
        }
      ]
    }
  }
}
