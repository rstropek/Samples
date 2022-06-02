targetScope = 'subscription'

@description('Name of the project, uses VatCalc by default')
param projectName string = 'VatCalc'

@description('Location of the resources, uses westeurope by default')
param location string = 'westeurope'

@allowed([
  'dev'
  'test'
  'prod'
])
@description('Stage of the deployment, uses dev by default')
param stage string = 'dev'

resource rg 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: '${projectName}-${stage}'
  location: location
  tags: {
    Project: projectName
    Environment: stage
  }
}

output rgName string = rg.name
