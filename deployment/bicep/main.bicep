targetScope = 'subscription'

@description('The environment name. Used for resource naming.')
param environmentName string = 'prod'

@description('The Azure region to deploy to.')
param location string = 'northeurope'

var resourceGroupName = 'WidgetCoStore-${environmentName}'
var tags = {
  environment: environmentName
  project: 'WidgetCoStore'
}

// Create Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

// Deploy Storage Resources
module storage 'modules/storage.bicep' = {
  name: 'storage-deployment'
  scope: rg
  params: {
    location: location
    environmentName: environmentName
    tags: tags
  }
}

// Deploy SQL Resources
module sql 'modules/sql.bicep' = {
  name: 'sql-deployment'
  scope: rg
  params: {
    location: location
    environmentName: environmentName
    tags: tags
    administratorLogin: 'WidgetCoAdmin'
    // Note: In production, use Key Vault or other secure method
    administratorLoginPassword: 'YourSecurePassword123!' 
  }
}

// Deploy App Service Resources
module appService 'modules/app-service.bicep' = {
  name: 'app-service-deployment'
  scope: rg
  params: {
    location: location
    environmentName: environmentName
    tags: tags
    storageAccountName: storage.outputs.storageAccountName
    storageAccountConnectionString: storage.outputs.storageConnectionString
    sqlServerName: sql.outputs.sqlServerName
    sqlDatabaseName: sql.outputs.sqlDatabaseName
    sqlAdminLogin: 'WidgetCoAdmin'
    sqlAdminPassword: 'YourSecurePassword123!'
  }
}