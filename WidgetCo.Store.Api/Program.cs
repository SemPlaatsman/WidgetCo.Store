using Microsoft.Extensions.Azure;
using Microsoft.Azure.Functions.Worker;
using Azure.Identity;
using Microsoft.Extensions.Hosting;
using WidgetCo.Store.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add Functions worker
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.ConfigureFunctionsWebApplication(workerBuilder =>
{
    workerBuilder.UseMiddleware<CustomMiddleware>();
});

// Add core services
builder.Services.AddControllers();

// Configure routing to ensure all endpoints are under /api
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Add application services and configuration
builder.Services.AddApplicationDatabase(builder.Configuration);
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();

// Add Azure services
builder.Services.AddAzureClients(clientBuilder =>
{
    var storageConnection = builder.Configuration["AzureStorage:ConnectionString"];
    clientBuilder.AddBlobServiceClient(storageConnection!)
        .WithCredential(new DefaultAzureCredential());
    clientBuilder.AddQueueServiceClient(storageConnection!)
        .WithCredential(new DefaultAzureCredential());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure middleware pipeline
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Configure endpoints
app.MapControllers();

// If not already set in launchSettings.json, set the URLs
if (app.Environment.IsDevelopment())
{
    app.Urls.Add("http://localhost:7164");
    app.Urls.Add("https://localhost:7165");
}

await app.RunAsync();