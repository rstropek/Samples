targetScope = 'subscription'

@description('Name of the project')
param projectName string

@description('Location of the resources, uses westeurope by default')
param location string = 'westeurope'

resource rg 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: projectName
  location: location
  tags: {
    Project: projectName
  }
}

output rgName string = rg.name
