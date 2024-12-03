param projectName string
param modelName string = 'gpt-4o'
param modelVersion string = '2024-08-06'
param raiPolicyName string

var abbrs = loadJsonContent('abbreviations.json')

// OpenAI account
resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' existing = {
  name: '${abbrs.cognitiveServicesAccounts}${uniqueString(projectName)}'
}

// Add a model deployment.
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  name: 'oai-gpt-4'
  parent: account
  sku: {
    capacity: 10 // capacity in thousands of TPM
    name: 'Standard'
  }
  properties: {
    model: {
      name: modelName
      version: modelVersion
      format: 'OpenAI'
    }
    raiPolicyName: raiPolicyName
  }
}
