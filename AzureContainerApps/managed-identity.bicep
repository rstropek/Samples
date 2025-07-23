param location string = resourceGroup().location
param projectName string
param stage string
param tags object

var abbrs = loadJsonContent('abbreviations.json')

// Create a user-assigned managed identity.
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: '${abbrs.managedIdentityUserAssignedIdentities}cr-${uniqueString(projectName, stage)}'
  location: location
  tags: tags
}

output managedIdName string = identity.name
output managedIdPrincipalId string = identity.properties.principalId
output managedIdClientId string = identity.properties.clientId
