param location string = resourceGroup().location
param projectName string
param stage string
param tags object
param managedIdPrincipalId string

var abbrs = loadJsonContent('abbreviations.json')
var roles = loadJsonContent('azure-roles.json')

// Create container registry
resource registry 'Microsoft.ContainerRegistry/registries@2024-11-01-preview' = {
  name: '${abbrs.containerRegistryRegistries}${uniqueString(projectName, stage)}'
  location: location
  tags: tags
  sku: {
    name: 'Basic' // Choose a different SKU if needed.
                  // Consider making this a parameter if you need more flexibility.
  }
  properties: {}
}

// TODO: Assign the 'AcrPush', 'AcrDelete', and 'AcrPull' roles to user group who need it.
// resource registryPushAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(registry.id, adminPrincipalId, 'push')
//   scope: registry
//   properties: {
//     principalId: ADD_PRINCIPAL_ID_HERE // Replace with the actual principal ID
//     roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roles.AcrPush)
//   }
// }

// Assign the 'AcrPull' role to the managed identity.
resource registryPullAssignmentManagedIdentity 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(registry.id, '-pull')
  scope: registry
  properties: {
    principalId: managedIdPrincipalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roles.AcrPull)
  }
}

output registryName string = registry.name
