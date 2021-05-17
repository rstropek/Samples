@description('Base name of the project. All resource names will be derived from that.')
param baseName string = 'tor21r'

@description('Custom domain name')
param customHostname string = 'techorama2021.coderdojo.net'

var functionName = '${baseName}-live-demo'

resource function 'Microsoft.Web/sites@2020-12-01' existing = {
  name: functionName
  resource functionCustomDomain 'hostNameBindings@2020-12-01' = {
    name: '${customHostname}'
  }
}
