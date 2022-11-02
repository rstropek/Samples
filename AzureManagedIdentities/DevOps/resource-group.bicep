// This script creates the resource group

targetScope = 'subscription'

@description('Name of the project, uses azmgedid by default')
param projectName string = 'azmgedid'

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
