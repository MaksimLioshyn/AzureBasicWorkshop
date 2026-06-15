targetScope = 'resourceGroup'

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Short prefix used for naming resources.')
param prefix string = 'azdemo'

@description('Linux App Service plan SKU name (for example B1, P1v3).')
param appServiceSkuName string = 'B1'

@description('Container image name and tag in ACR repository, for example webapp:1.0.0.')
param containerImage string = 'webapp:latest'

@description('Container registry name. Must be globally unique and use only lowercase letters and numbers.')
param containerRegistryName string

@description('Microsoft Entra tenant ID used by the web app.')
param entraTenantId string

@description('App registration client ID used by OpenID Connect login.')
param entraClientId string

@description('OpenID Connect callback path. Default value works for ASP.NET Core.')
param entraCallbackPath string = '/signin-oidc'

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: containerRegistryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
    anonymousPullEnabled: false
    publicNetworkAccess: 'Enabled'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${prefix}-plan'
  location: location
  sku: {
    name: appServiceSkuName
    tier: startsWith(appServiceSkuName, 'P') ? 'PremiumV3' : 'Basic'
    size: appServiceSkuName
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: '${prefix}-webapp'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOCKER|${acr.properties.loginServer}/${containerImage}'
      acrUseManagedIdentityCreds: true
      appSettings: [
        {
          name: 'WEBSITES_PORT'
          value: '8080'
        }
        {
          name: 'AzureAd__Instance'
          value: 'https://login.microsoftonline.com/'
        }
        {
          name: 'AzureAd__TenantId'
          value: entraTenantId
        }
        {
          name: 'AzureAd__ClientId'
          value: entraClientId
        }
        {
          name: 'AzureAd__CallbackPath'
          value: entraCallbackPath
        }
      ]
    }
  }
}

resource acrPullRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(webApp.id, 'AcrPull')
  scope: acr
  properties: {
    principalId: webApp.identity.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalType: 'ServicePrincipal'
  }
}

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output registryLoginServer string = acr.properties.loginServer
