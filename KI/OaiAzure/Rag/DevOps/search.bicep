param location string = resourceGroup().location

param projectName string

param tags object

param sku string = 'standard'

var abbrs = loadJsonContent('abbreviations.json')

param principalIds array

param peSubnetId string

param dnsZoneId string

param searchServiceName string

var roleIds = {
  // See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#all
  SearchServiceContributor: '7ca78c08-252a-4471-8644-bb5ff32d4ba0'
  SearchIndexDataContributor: '8ebe5a00-799e-43f5-93ac-243d3dce84a7'
  SearchIndexDataReader: '1407120a-92aa-4202-b7e9-c0e197c71c8f'
}

resource search 'Microsoft.Search/searchServices@2025-05-01' = {
  name: '${abbrs.searchSearchServices}${uniqueString(projectName)}'
  location: location
  tags: tags
  sku: {
    name: sku
  }
  properties: {
    replicaCount: 1
    partitionCount: 1
    hostingMode: 'default'
    disableLocalAuth: true
    publicNetworkAccess: 'Disabled'
  }
}

// Create private endpoint for the Search Service
resource privateEndpoint 'Microsoft.Network/privateEndpoints@2024-03-01' = {
  name: searchServiceName
  location: location
  tags: tags
  properties: {
    subnet: {
      id: peSubnetId
    }
    privateLinkServiceConnections: [
      {
        name: '${abbrs.networkPrivateEndpoints}${uniqueString(projectName, 'search')}'
        properties: {
          privateLinkServiceId: search.id
          groupIds: [
            'searchService'
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

resource roleAssignments1 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for p in principalIds: {
  name: guid(p, search.id, '1')
  scope: search
  properties: {
    principalId: p
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.SearchIndexDataContributor)
  }
}]
resource roleAssignments2 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for p in principalIds: {
  name: guid(p, search.id, '2')
  scope: search
  properties: {
    principalId: p
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.SearchIndexDataReader)
  }
}]
resource roleAssignments3 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for p in principalIds: {
  name: guid(p, search.id, '3')
  scope: search
  properties: {
    principalId: p
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleIds.SearchServiceContributor)
  }
}]

output searchServiceName string = search.name
