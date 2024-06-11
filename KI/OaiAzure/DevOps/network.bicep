param location string = resourceGroup().location

param projectName string

param tags object

var abbrs = loadJsonContent('abbreviations.json')

resource privateVnet 'Microsoft.Network/virtualNetworks@2021-08-01' = {
  name: '${abbrs.networkVirtualNetworks}${uniqueString(projectName)}'
  location: location
  tags: tags
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '10.0.10.0/24'
        }
      }
      {
        // Subnet for private endpoints
        name: 'privateendpoints'
        properties: {
          addressPrefix: '10.0.11.0/24'
          privateEndpointNetworkPolicies: 'Enabled'
          privateLinkServiceNetworkPolicies: 'Enabled'
        }
      }
      {
        // Subnet for webapps
        name: 'webapps'
        properties: {
          addressPrefix: '10.0.12.0/24'
          delegations: [
            {
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
    ]
  }
}

// Create a private DNS zone for the private link to Azure OpenAI
resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: 'privatelink.openai.azure.com'
  location: 'global'
  properties: {}
  dependsOn: [
    privateVnet
  ]
}

// Link the private DNS zone to the virtual network
resource privateDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  parent: privateDnsZone
  name: '${privateDnsZone.name}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: privateVnet.id
    }
  }
}

output webSubnetId string = privateVnet.properties.subnets[2].id
output peSubnetId string = privateVnet.properties.subnets[1].id
output privateDnsZoneId string = privateDnsZone.id
