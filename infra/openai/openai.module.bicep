targetScope = 'resourceGroup'

@description('')
param location string = resourceGroup().location

@description('')
param principalId string

@description('')
param principalType string

@description('')
param userId string

resource cognitiveServicesAccount_wXAGTFUId 'Microsoft.CognitiveServices/accounts@2023-05-01' = {
  name: toLower(take('openai${uniqueString(resourceGroup().id)}', 24))
  location: location
  kind: 'OpenAI'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: toLower(take(concat('openai', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
}

resource roleAssignment_Hsk8rxWY8 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: cognitiveServicesAccount_wXAGTFUId
  name: guid(cognitiveServicesAccount_wXAGTFUId.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd') // Cognitive Services User role
    principalId: principalId
    principalType: principalType
  }
}

resource roleAssignment_Hsk8rxWY9 'Microsoft.Authorization/roleAssignments@2022-04-01' =  if (!empty(userId)) {
  scope: cognitiveServicesAccount_wXAGTFUId
  name: guid(cognitiveServicesAccount_wXAGTFUId.id, userId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd'))
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '5e0bd9bd-7b93-4f28-af87-19fc36ad61bd') // Cognitive Services User role
    principalId: userId
    principalType: 'user'
  }
}

resource cognitiveServicesAccountDeployment_6E9woetGC 'Microsoft.CognitiveServices/accounts/deployments@2023-05-01' = {
  parent: cognitiveServicesAccount_wXAGTFUId
  name: 'chat'
  sku: {
    name: 'GlobalStandard'
    capacity: 10
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o'
      version: '2024-05-13'
    }
  }
}

output connectionString string = 'Endpoint=${cognitiveServicesAccount_wXAGTFUId.properties.endpoint}'
