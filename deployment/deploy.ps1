# Variables
$ResourceGroup = "WidgetCoStore"
$Location = "northeurope"  # or your preferred region
$StorageAccount = "widgetcostorage123"  # must be globally unique
$SqlServer = "widgetco-sql-123"  # must be globally unique
$SqlDatabase = "WidgetCoStore"
$AppServicePlan = "widgetco-plan"
$WebApp = "widgetco-api-123"  # must be globally unique
$FunctionApp = "widgetco-functions-123"  # must be globally unique

# Admin credentials for SQL Server
$SqlAdminUsername = "WidgetCoAdmin"
$SqlAdminPassword = "YourSecurePassword123!"  # Change this!

# Login to Azure
az login

# Register required resource providers
Write-Host "Registering resource providers..."
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.Sql
az provider register --namespace Microsoft.Storage

# Create Resource Group
Write-Host "Creating Resource Group..."
az group create --name $ResourceGroup --location $Location

# Create Storage Account
Write-Host "Creating Storage Account..."
az storage account create `
    --name $StorageAccount `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku Standard_LRS `
    --allow-blob-public-access false

# Get storage connection string
$StorageConnectionString = $(az storage account show-connection-string --name $StorageAccount --resource-group $ResourceGroup --query connectionString -o tsv)

# Create SQL Server
Write-Host "Creating SQL Server..."
az sql server create `
    --name $SqlServer `
    --resource-group $ResourceGroup `
    --location $Location `
    --admin-user $SqlAdminUsername `
    --admin-password $SqlAdminPassword

# Configure SQL Server firewall
Write-Host "Configuring SQL Server firewall..."
az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name "AllowAzureServices" `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

# Add your current IP to SQL Server firewall
$CurrentIP = $(Invoke-WebRequest -Uri "https://api.ipify.org" -UseBasicParsing).Content
az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name "AllowCurrentIP" `
    --start-ip-address $CurrentIP `
    --end-ip-address $CurrentIP

# Create SQL Database
Write-Host "Creating SQL Database..."
az sql db create `
    --name $SqlDatabase `
    --server $SqlServer `
    --resource-group $ResourceGroup `
    --service-objective Basic

# Create App Service Plan
Write-Host "Creating App Service Plan..."
az appservice plan create `
    --name $AppServicePlan `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku P1V2 `
    --is-linux false

# Create Web App
Write-Host "Creating Web App..."
az webapp create `
    --name $WebApp `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --runtime "dotnet:8"

# Create Function App
Write-Host "Creating Function App..."
az functionapp create `
    --name $FunctionApp `
    --storage-account $StorageAccount `
    --resource-group $ResourceGroup `
    --plan $AppServicePlan `
    --runtime dotnet-isolated `
    --runtime-version 8 `
    --functions-version 4 `
    --os-type Windows

# Configure Web App Settings
Write-Host "Configuring Web App settings..."
az webapp config appsettings set `
    --name $WebApp `
    --resource-group $ResourceGroup `
    --settings `
    "Api:ReturnDetailedErrors=false" `
    "OrderStorage:QueueName=order-processing" `
    "ReviewStorage:TableName=ProductReviews" `
    "ProductImageStorage:ContainerName=product-images" `
    "OrderStorage:ConnectionString=$StorageConnectionString" `
    "ReviewStorage:ConnectionString=$StorageConnectionString" `
    "ProductImageStorage:ConnectionString=$StorageConnectionString" `
    "ConnectionStrings:SqlServerConnectionString=Server=$SqlServer.database.windows.net;Database=$SqlDatabase;User Id=$SqlAdminUsername;Password=$SqlAdminPassword"

# Configure Function App Settings
Write-Host "Configuring Function App settings..."
az functionapp config appsettings set `
    --name $FunctionApp `
    --resource-group $ResourceGroup `
    --settings `
    "FUNCTIONS_WORKER_RUNTIME=dotnet-isolated" `
    "AzureWebJobsStorage=$StorageConnectionString" `
    "Api:ReturnDetailedErrors=false" `
    "OrderStorage:QueueName=order-processing" `
    "ReviewStorage:TableName=ProductReviews" `
    "ProductImageStorage:ContainerName=product-images" `
    "OrderStorage:ConnectionString=$StorageConnectionString" `
    "ReviewStorage:ConnectionString=$StorageConnectionString" `
    "ProductImageStorage:ConnectionString=$StorageConnectionString" `
    "ConnectionStrings:SqlServerConnectionString=Server=$SqlServer.database.windows.net;Database=$SqlDatabase;User Id=$SqlAdminUsername;Password=$SqlAdminPassword"

Write-Host "Deployment completed!"
Write-Host "Web App URL: https://$WebApp.azurewebsites.net"
Write-Host "Function App URL: https://$FunctionApp.azurewebsites.net"
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Update your application's connection strings"
Write-Host "2. Deploy your application code"
Write-Host "3. Run your database migrations"
Write-Host "4. Test your endpoints"