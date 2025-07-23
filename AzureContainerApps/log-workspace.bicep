param location string = resourceGroup().location
param projectName string
param stage string
param tags object

var abbrs = loadJsonContent('abbreviations.json')

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: '${abbrs.operationalInsightsWorkspaces}${uniqueString(projectName, stage)}'
  location: location
  tags: tags
  properties: {
    // Consider turning off public network access for ingestion if not needed.
    // Depends on the project's requirements.
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    retentionInDays: 30
    features: {
      disableLocalAuth: false
      enableDataExport: false
    }
    sku: {
      name: 'PerGB2018'
    }
  }
}

output workspaceName string = logAnalytics.name
output workspaceId string = logAnalytics.id
