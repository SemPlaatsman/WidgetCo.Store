#!/bin/bash

# Variables
RESOURCE_GROUP="WidgetCoStore"
LOCATION="northeurope"  # or your preferred region
STORAGE_ACCOUNT="widgetcostorage123"  # must be globally unique
SQL_SERVER="widgetco-sql-123"  # must be globally unique
SQL_DATABASE="WidgetCoStore"
APP_SERVICE_PLAN="widgetco-plan"
WEB_APP="widgetco-api-123"  # must be globally unique
FUNCTION_APP="widgetco-functions-123"  # must be globally unique

# Admin credentials for SQL Server
SQL_ADMIN_USERNAME="WidgetCoAdmin"
SQL_ADMIN_PASSWORD="YourSecurePassword123!"  # Change this!

# Login to Azure
az login

# Register required resource providers
echo "Registering resource providers..."
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.Sql
az provider register --namespace Microsoft.Storage

# Create Resource Group
echo "Creating Resource Group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create Storage Account
echo "Creating Storage Account..."
az storage account create \
    --name $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku Standard_LRS \
    --allow-blob-public-access false

# Get storage connection string
STORAGE_CONNECTION_STRING=$(az storage account show-connection-string --name $STORAGE_ACCOUNT --resource-group $RESOURCE_GROUP --query connectionString -o tsv)

# Create SQL Server
echo "Creating SQL Server..."
az sql server create \
    --name $SQL_SERVER \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USERNAME \
    --admin-password $SQL_ADMIN_PASSWORD

# Configure SQL Server firewall
echo "Configuring SQL Server firewall..."
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER \
    --name "AllowAzureServices" \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0

# Add your current IP to SQL Server firewall
CURRENT_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER \
    --name "AllowCurrentIP" \
    --start-ip-address $CURRENT_IP \
    --end-ip-address $CURRENT_IP

# Create SQL Database
echo "Creating SQL Database..."
az sql db create \
    --name $SQL_DATABASE \
    --server $SQL_SERVER \
    --resource-group $RESOURCE_GROUP \
    --service-objective Basic

# Create App Service Plan
echo "Creating App Service Plan..."
az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku P1V2 \
    --is-linux false

# Create Web App
echo "Creating Web App..."
az webapp create \
    --name $WEB_APP \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --runtime "dotnet:8"

# Create Function App
echo "Creating Function App..."
az functionapp create \
    --name $FUNCTION_APP \
    --storage-account $STORAGE_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --runtime dotnet-isolated \
    --runtime-version 8 \
    --functions-version 4 \
    --os-type Windows

# Configure Web App Settings
echo "Configuring Web App settings..."
az webapp config appsettings set \
    --name $WEB_APP \
    --resource-group $RESOURCE_GROUP \
    --settings \
    "Api:ReturnDetailedErrors=false" \
    "OrderStorage:QueueName=order-processing" \
    "ReviewStorage:TableName=ProductReviews" \
    "ProductImageStorage:ContainerName=product-images" \
    "OrderStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ReviewStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ProductImageStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ConnectionStrings:SqlServerConnectionString=Server=$SQL_SERVER.database.windows.net;Database=$SQL_DATABASE;User Id=$SQL_ADMIN_USERNAME;Password=$SQL_ADMIN_PASSWORD"

# Configure Function App Settings
echo "Configuring Function App settings..."
az functionapp config appsettings set \
    --name $FUNCTION_APP \
    --resource-group $RESOURCE_GROUP \
    --settings \
    "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated" \
    "AzureWebJobsStorage=$STORAGE_CONNECTION_STRING" \
    "Api:ReturnDetailedErrors=false" \
    "OrderStorage:QueueName=order-processing" \
    "ReviewStorage:TableName=ProductReviews" \
    "ProductImageStorage:ContainerName=product-images" \
    "OrderStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ReviewStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ProductImageStorage:ConnectionString=$STORAGE_CONNECTION_STRING" \
    "ConnectionStrings:SqlServerConnectionString=Server=$SQL_SERVER.database.windows.net;Database=$SQL_DATABASE;User Id=$SQL_ADMIN_USERNAME;Password=$SQL_ADMIN_PASSWORD"

echo "Deployment completed!"
echo "Web App URL: https://$WEB_APP.azurewebsites.net"
echo "Function App URL: https://$FUNCTION_APP.azurewebsites.net"
echo ""
echo "Next steps:"
echo "1. Update your application's connection strings"
echo "2. Deploy your application code"
echo "3. Run your database migrations"
echo "4. Test your endpoints"