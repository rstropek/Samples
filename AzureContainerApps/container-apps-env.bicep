param location string = resourceGroup().location
param projectName string
param stage string
param tags object
param logAnalyticsWorkspaceName string

var abbrs = loadJsonContent('abbreviations.json')

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: logAnalyticsWorkspaceName
}

// Create container apps environment
resource environment 'Microsoft.App/managedEnvironments@2025-01-01' = {
  name: '${abbrs.appManagedEnvironments}${uniqueString(projectName, stage)}'
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
}

output environmentName string = environment.name
