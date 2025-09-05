targetScope = 'subscription'

@description('Name of the project, uses oaiazure by default')
param projectName string = 'oaiazure'

@description('Location of the resources, uses Sweden Central by default')
param location string = 'swedencentral'

resource rg 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: projectName
  location: location
  tags: {
    Project: projectName
  }
}

output rgName string = rg.name
output subscriptionId string = subscription().subscriptionId
