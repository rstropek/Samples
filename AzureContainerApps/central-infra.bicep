param location string = resourceGroup().location
param projectName string
param stage string
param tags object
param managedIdPrincipalId string

var infraTags = union(tags, {
  Type: 'central-infrastructure'
})

module acrModule './container-registry.bicep' = {
  name: '${deployment().name}-container-registry'
  params: {
    location: location
    projectName: projectName
    stage: stage
    managedIdPrincipalId: managedIdPrincipalId
    tags: infraTags
  }
}

module workspaceModule './log-workspace.bicep' = {
  name: '${deployment().name}-container-apps-env'
  params: {
    location: location
    projectName: projectName
    stage: stage
    tags: infraTags
  }
}

module containerAppsEnv './container-apps-env.bicep' = {
  name: '${deployment().name}-workspace'
  params: {
    location: location
    projectName: projectName
    stage: stage
    tags: infraTags
    logAnalyticsWorkspaceName: workspaceModule.outputs.workspaceName
  }
}

output registryName string = acrModule.outputs.registryName
output workspaceName string = workspaceModule.outputs.workspaceName
output workspaceId string = workspaceModule.outputs.workspaceId
output environmentName string = containerAppsEnv.outputs.environmentName
