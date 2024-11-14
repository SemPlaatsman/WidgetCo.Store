using Microsoft.Extensions.Azure;
using WidgetCo.Store.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add application services and configuration
builder.Services.AddApplicationDatabase(builder.Configuration);
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();

// Add Azure services
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureStorage:ConnectionString"]!, preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();