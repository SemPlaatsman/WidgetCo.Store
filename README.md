# Widget & Co Store

> [!NOTE]
> <br>This project is also hosted on GitHub. See the repository [here](https://github.com/SemPlaatsman/WidgetCo.Store)

This is a proof of concept project demonstrating cloud database usage in Azure. The solution consists of an API project and an Azure Functions project, utilizing various Azure services including SQL Database, Table Storage, Queue Storage, and Blob Storage.

## Deployment Status

A deployment attempt was made with the following endpoints:

- API URL: https://widgetco-api-gceebgf0f0cyceaw.northeurope-01.azurewebsites.net/
- Functions URL: https://widgetco-functions.azurewebsites.net/

However, some issues were encountered during deployment and configuration:
- Functions were not accessible through their endpoints
- Some configuration settings may need adjustment
- Further investigation is needed for proper service communication

These issues are typical in a proof of concept project and demonstrate the real-world challenges of cloud deployment and configuration.

## Architecture
```mermaid
flowchart TB
    subgraph Client
        API[Web API]
    end

    subgraph Azure Functions
        RF[Review Functions]
        IF[Image Functions]
        OF[Order Processing Function]
    end

    subgraph Storage
        subgraph "Core Business Data"
            SQL[(SQL Database)]
            BLOB[Blob Storage]
        end
        subgraph "Processing & Reviews"
            QUEUE[Storage Queue]
            REVIEW_TABLE[Review Table Storage]
        end
    end

    subgraph "SQL Database Tables"
        PRODUCTS[Products]
        ORDERS[Orders]
    end

    API --> RF
    API --> IF
    API --> SQL
    API --> QUEUE
    RF --> REVIEW_TABLE
    IF --> BLOB
    OF --> SQL
    QUEUE --> OF
    SQL --- PRODUCTS
    SQL --- ORDERS
```

<details>
<summary>If the mermaid file does not render in your Markdown renderer, open this to show the PNG!</summary>

![Architecture PNG](./docs/architecture.png)

</details>

You can also checkout the mermaid chart file [here](./docs/architecture.mmd). 

## Project Structure

- `Api/` - Web API project
- `Functions/` - Azure Functions project
  - `OrderProcessingFunction` - Handles order processing from queue
  - `ProductImageFunction` - Manages product images
  - `ReviewFunction` - Manages product reviews
- `Core/` - Core domain logic
- `Infrastructure/` - Implementation logic and services
- `deployment/` - Azure deployment scripts

## API Endpoints

See also:
- [orders.http](./docs/http/orders.http)
- [products.http](./docs/http/products.http)

### Products API
- `[GET] /api/products` - Get all products
- `[GET] /api/products/{productId}` - Get a specific product
- `[POST] /api/products` - Create a new product
- `[PUT] /api/products/{productId}` - Update an existing product

### Orders API
- `[POST] /api/orders` - Create a new order
- `[GET] /api/orders/{orderRequestId}` - Get order status
- `[POST] /api/orders/{orderId}/ship` - Ship an order

## Function Endpoints
See also:
- [images.http](./docs/http/images.http)
- [reviews.http](./docs/http/reviews.http)

### Review Functions
- `[POST] /api/reviews` - Add a new product review
- `[GET] /api/reviews/{productId}` - Get reviews for a specific product

### Product Image Functions
- `[GET] /api/images` - Get all product images
- `[POST] /api/images` - Upload a new product image

### Order Processing
- Queue trigger function that processes orders from the order-processing queue

## API Testing

### HTTP Files
The [`docs/http`](./docs/http) folder contains HTTP files for testing the API endpoints using tools like Visual Studio Code's REST Client or JetBrains' HTTP Client:
- [`products.http`](./docs/http/products.http) - Product-related endpoints
- [`orders.http`](./docs/http/orders.http) - Order-related endpoints
- [`images.http`](./docs/http/images.http) - (Product) Image-related endpoints
- [`reviews.http`](./docs/http/reviews.http) - Review-related endpoints

### Insomnia Export
For testing with Insomnia REST Client, an export file is provided in the [`docs/insomnia`](./docs/insomnia) folder. To use it:

1. Open Insomnia
2. Go to `Application > Preferences > Data > Import Data`
3. Select "From File"
4. Navigate to [`docs/insomnia/WidgetCo-Store.json`](./docs/insomnia/WidgetCo-Store.json)
5. All endpoints will be imported with:
   - Environment configurations (Development/Production)
   - Example requests
   - Request body templates

The export includes all API and Function endpoints with proper configurations for both local development and Azure environments.

## Azure Resources

This project uses the following Azure resources:
- Azure SQL Database (for main data storage)
  - Products table
  - Orders table
- Azure Table Storage (for product reviews)
- Azure Queue Storage (for order processing)
- Azure Blob Storage (for product images)
- Azure App Service (for hosting the API)
- Azure Functions (for serverless operations)

## Deployment

### Prerequisites
- Azure CLI installed
- PowerShell or Bash shell
- An Azure subscription
- .NET 8.0 SDK

### Deployment Scripts

The `deployment` folder contains scripts to set up all required Azure resources:
- [`deploy.ps1`](./deployment/deploy.ps1) - PowerShell deployment script
- [`deploy.sh`](./deployment/deploy.sh) - Bash deployment script

To deploy the infrastructure:

1. Navigate to the deployment folder:
```bash
cd deployment
```

2. Run either the PowerShell or Bash script:
```powershell
# PowerShell
.\deploy-azure.ps1
```
```bash
# Bash
./deploy-azure.sh
```

The scripts will:
1. Create a resource group
2. Set up all required Azure services
3. Configure connection strings and application settings
4. Set up proper authentication and security rules

### Manual Configuration

After running the deployment scripts:
1. Update your application's connection strings in both projects
2. Deploy your application code through Visual Studio or GitHub Actions
3. Run database migrations
4. Test the endpoints

## Development

### Local Development Prerequisites
- Visual Studio 2022
- .NET 8.0 SDK
- Azure Storage Emulator
- SQL Server (LocalDB or full installation)

### Configuration Files
- `appsettings.json` - API project configuration
- `local.settings.json` - Functions project configuration

### Local Development Setup
1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Update connection strings in configuration files
4. Run database migrations
5. Start both the API and Functions projects

## Cloud Databases Demonstration

This project showcases various cloud database technologies:
1. **Relational Data** - Azure SQL Database for structured data
2. **NoSQL Data** - Table Storage for product reviews
3. **Queue Storage** - For asynchronous order processing
4. **Blob Storage** - For storing product images

## Notes

- This is a proof of concept project
- Some security features are simplified for demonstration
- Local development uses development storage emulators
- Production deployment uses full Azure services
- Deployment issues were encountered and documented for learning purposes