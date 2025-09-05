param location string = resourceGroup().location

param projectName string

param tags object

param sku string = 'S0'

param peSubnetId string

param dnsZoneId string

param principalIds array

var abbrs = loadJsonContent('abbreviations.json')

var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  cognitiveServicesUser: 'a97b65f3-24c7-4388-baec-2e87135dc908'
}

// Create OpenAI account
resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: '${abbrs.cognitiveServicesAccounts}${uniqueString(projectName)}'
  location: location
  tags: tags
  kind: 'OpenAI'
  properties: {
    publicNetworkAccess: 'Disabled' // Only allow access from private endpoints
    disableLocalAuth: true // Disable local authentication (i.e. no API key, only managed identity)
    customSubDomainName: uniqueString(projectName)
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  sku: {
    name: sku
  }
}

// Create private endpoint for the OpenAI account
resource privateEndpoint 'Microsoft.Network/privateEndpoints@2024-03-01' = {
  name: '${abbrs.networkPrivateEndpoints}${uniqueString(projectName)}'
  location: location
  tags: tags
  properties: {
    subnet: {
      id: peSubnetId
    }
    privateLinkServiceConnections: [
      {
        name: '${abbrs.networkPrivateEndpoints}${uniqueString(projectName)}'
        properties: {
          privateLinkServiceId: account.id
          groupIds: [
            'account'
          ]
        }
      }
    ]
  }
}

resource pvtEndpointDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2024-03-01' = {
  name: 'pvtEndpointDnsGroup'
  parent: privateEndpoint
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'privatelink-openai-azure-com'
        properties: {
          privateDnsZoneId: dnsZoneId
        }
      }
    ]
  }
}

resource roleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for p in principalIds: {
  name: guid(p, account.id)
  scope: account
  properties: {
    principalId: p
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.cognitiveServicesUser)
  }
}]

output accountName string = account.name
