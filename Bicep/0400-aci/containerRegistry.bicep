@description('Name of the project')
param projectName string

@description('Location of the resources, uses resource group\'s location by default')
param location string = resourceGroup().location

@description('ID of admin user')
param adminPrincipalId string

var baseName = projectName
var names = {
  registry: 'cr${uniqueString('${baseName}')}'
  identity: 'mi-${uniqueString('${baseName}')}'
}
var tags = {
  Project: projectName
}
var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  AcrDelete: 'c2f4ef07-c644-48eb-af81-4b1b4947fb11'
  AcrPush: '8311e382-0749-4cb8-b61a-304f252e45ec'
  AcrPull: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
}

resource registry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' = {
  name: names.registry
  location: location
  tags: tags
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: names.identity
  location: location
  tags: tags
}

resource registryPushAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(registry.id, adminPrincipalId, 'delete')
  scope: registry
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.AcrPush)
  }
}

resource registryDeleteAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(registry.id, adminPrincipalId, 'push')
  scope: registry
  properties: {
    principalId: adminPrincipalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.AcrDelete)
  }
}

resource registryPullAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(names.registry, '-aci-pull')
  scope: registry
  properties: {
    principalId: identity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.AcrPull)
  }
}

output registryName string = registry.name
