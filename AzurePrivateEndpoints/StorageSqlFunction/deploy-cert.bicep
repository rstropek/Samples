@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'tor21r'

@description('Custom domain name')
param customHostname string = 'techorama2021.coderdojo.net'

var functionName = '${baseName}-live-demo'

resource hosting 'Microsoft.Web/serverfarms@2020-12-01' existing = {
  name: 'app-${uniqueString(baseName)}'
}

resource cert 'Microsoft.Web/certificates@2020-12-01' = {
  name: '${functionName}-cert'
  location: resourceGroup().location
  properties: {
    serverFarmId: hosting.id
    canonicalName: customHostname
  }
}

output certThumbprint string = cert.properties.thumbprint
