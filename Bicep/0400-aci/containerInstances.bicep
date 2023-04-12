@description('Name of the project')
param projectName string

@description('Location of the resources, uses resource group\'s location by default')
param location string = resourceGroup().location

@description('Number of students')
param studentCount int = 5

@description('Name of the container image')
param containerImage string = 'teachingwithazure'

var baseName = projectName
var names = {
  identity: 'mi-${uniqueString('${baseName}')}'
  registry: 'cr${uniqueString('${baseName}')}'
  instance: 'ci-${uniqueString('${baseName}')}'
}
var tags = {
  Project: projectName
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: names.identity
}

resource registry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' existing = {
  name: names.registry
}

resource containerGroups 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, studentCount): {
  name: '${names.instance}-${i}'
  location: location
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    containers: [
      {
        name: 'container'
        properties: {
          image: '${registry.name}.azurecr.io/${containerImage}:latest'
          resources: {
            requests: {
              cpu: 2
              memoryInGB: 4
            }
          }
          ports: [
            {
              port: 22
              protocol: 'TCP'
            }
            {
              port: 80
              protocol: 'TCP'
            }
          ]
        }
      }
    ]
    ipAddress: {
      dnsNameLabel: '${names.instance}-${i}'
      type: 'Public'
      ports: [
        {
          port: 22
          protocol: 'TCP'
        }
        {
          port: 80
          protocol: 'TCP'
        }
      ]
    }
    imageRegistryCredentials: [
      {
        identity: identity.id
        server: '${registry.name}.azurecr.io'
      }
    ]
    osType: 'Linux'
    restartPolicy: 'Always'
  }
}]
