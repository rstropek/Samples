// NOTE that chaning the project name requires changing of 
// build scripts!
@description('Name of the project, uses VatCalc by default')
param projectName string = 'VatCalc'

@description('Location of the resources, uses resource group\'s location by default')
param location string = resourceGroup().location

@allowed([
  'dev'
  'test'
  'prod'
])
@description('Stage of the deployment, uses dev by default')
param stage string = 'dev'

@allowed([
  'P1V2'
  'P2V2'
  'P3V2'
])
param appServicePlanSku string = 'P1V2'

@description('Indicates whether IP addresses in logs should be masked, consider GDPR before enabling this')
param disableIpMaskingInLogs bool = false

@description('Name of the Azure DevOps Repository containing the web app\'s source code')
param devopsRepositoryUrl string

@description('Name of the Azure DevOps Branch containing the web app\'s source code, uses main as default')
param devopsBranch string = 'main'

var baseName = '${stage}-${projectName}'
var names = {
  appServicePlan: 'plan-${uniqueString('${baseName}')}'
  planStorage: 'st${uniqueString('${baseName}-planstorage')}'
  appInsights: 'appi-${uniqueString('${baseName}')}'
  logAnalytics: 'log-${uniqueString('${baseName}')}'
  apiApp: 'app-${uniqueString('${baseName}-api')}'
  frontendApp: 'app-${uniqueString('${baseName}-frontend')}'
  webApp: 'stapp-${uniqueString('${baseName}-frontend')}'
}
var tags = {
  Project: projectName
  Environment: stage
}
var defaultSiteConfig = {
  alwaysOn: true
  ftpsState: 'Disabled'
  minTlsVersion: '1.2'
  scmMinTlsVersion: '1.2'
  http20Enabled: true
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: names.appServicePlan
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true
  }
  kind: 'linux'
}

resource planStorage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: names.planStorage
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true // OK because this storage will not be used for any customer-related data
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: names.logAnalytics
  location: location
  tags: tags
  properties: {
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

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: names.appInsights
  location: location
  tags: union(tags, {
    'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${names.apiApp}': 'Resource'
  })
  kind: 'web'
  properties: {
    Application_Type: 'web'
    DisableIpMasking: disableIpMaskingInLogs
    WorkspaceResourceId: logAnalytics.id
  }
}

resource apiApp 'Microsoft.Web/sites@2021-03-01' = {
  name: names.apiApp
  location: location
  tags: union(tags, {
    Tier: 'Backend'
  })
  kind: 'app,linux'
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: union(defaultSiteConfig,{
      linuxFxVersion: 'DOTNETCORE|6.0'
    })
  }

  resource settings 'config@2021-03-01' = {
    name: 'appsettings'
    properties: {
      'APPLICATIONINSIGHTS_CONNECTION_STRING': appInsights.properties.ConnectionString
    }
  }
}

resource staticWebApp 'Microsoft.Web/staticSites@2020-12-01' = {
  name: names.webApp
  location: location
  tags: union(tags, {
    Tier: 'Frontend'
  })
  sku: {
    name: 'Standard'
  }
  properties: {
    // The provider, repositoryUrl and branch fields are required for successive deployments to succeed
    // for more details see: https://github.com/Azure/static-web-apps/issues/516
    provider: 'DevOps'
    repositoryUrl: devopsRepositoryUrl
    branch: devopsBranch
    buildProperties: {
      skipGithubActionWorkflowGeneration: true
    }
  }
}

output deploymentToken string = listSecrets(staticWebApp.id, staticWebApp.apiVersion).properties.apiKey
