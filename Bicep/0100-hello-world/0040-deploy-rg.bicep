// this file can only be deployed at a subscription scope
// Read more at https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/deploy-to-subscription?tabs=azure-cli
targetScope = 'subscription'

@description('Name of the Resource Group to create')
param rgName string = 'DemoResourceGroup'

@description('Location for the Resource Group')
param rgLocation string = 'westeurope'

var dept = 'IT'

resource demoRG 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: rgName
  location: rgLocation
  tags: {
    Dept: dept
    Environment: 'Test'
  }
}

output resourceID string = demoRG.id
