param modelName string = 'gpt-4.1'
param modelVersion string = '2025-04-14'
param raiPolicyName string
param accountName string

var abbrs = loadJsonContent('abbreviations.json')

// OpenAI account
resource account 'Microsoft.CognitiveServices/accounts@2023-05-01' existing = {
  name: accountName
}

// Add a model deployment.
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  name: modelName
  parent: account
  sku: {
    capacity: 5 // capacity in thousands of TPM
    name: 'GlobalStandard'
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
