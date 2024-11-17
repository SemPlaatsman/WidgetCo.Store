param location string
param environmentName string
param tags object
param storageAccountName string
param storageAccountConnectionString string
param sqlServerName string
param sqlDatabaseName string
param sqlAdminLogin string
@secure()
param sqlAdminPassword string

var appServicePlanName = 'widgetco-${environmentName}-plan'
var webAppName = 'widgetco-api-${environmentName}'
var functionAppName = 'widgetco-functions-${environmentName}'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  tags: tags
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'Api:ReturnDetailedErrors'
          value: 'false'
        }
        {
          name: 'OrderStorage:QueueName'
          value: 'order-processing'
        }
        {
          name: 'ReviewStorage:TableName'
          value: 'ProductReviews'
        }
        {
          name: 'ProductImageStorage:ContainerName'
          value: 'product-images'
        }
        {
          name: 'OrderStorage:ConnectionString'
          value: storageAccountConnectionString
        }
        {
          name: 'ReviewStorage:ConnectionString'
          value: storageAccountConnectionString
        }
        {
          name: 'ProductImageStorage:ConnectionString'
          value: storageAccountConnectionString
        }
      ]
      connectionStrings: [
        {
          name: 'SqlServerConnectionString'
          connectionString: 'Server=${sqlServerName}.database.windows.net;Database=${sqlDatabaseName};User Id=${sqlAdminLogin};Password=${sqlAdminPassword}'
          type: 'SQLServer'
        }
      ]
    }
  }
}

resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v8.0'
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccountConnectionString
        }
        {
          name: 'Api:ReturnDetailedErrors'
          value: 'false'
        }
        {
          name: 'OrderStorage:QueueName'
          value: 'order-processing'
        }
        {
          name: 'ReviewStorage:TableName'
          value: 'ProductReviews'
        }
        {
          name: 'ProductImageStorage:ContainerName'
          value: 'product-images'
        }
        {
          name: 'OrderStorage:ConnectionString'
          value: storageAccountConnectionString
        }
        {
          name: 'ReviewStorage:ConnectionString'
          value: storageAccountConnectionString
        }
        {
          name: 'ProductImageStorage:ConnectionString'
          value: storageAccountConnectionString
        }
      ]
      connectionStrings: [
        {
          name: 'SqlServerConnectionString'
          connectionString: 'Server=${sqlServerName}.database.windows.net;Database=${sqlDatabaseName};User Id=${sqlAdminLogin};Password=${sqlAdminPassword}'
          type: 'SQLServer'
        }
      ]
    }
  }
}

output webAppName string = webApp.name
output functionAppName string = functionApp.name