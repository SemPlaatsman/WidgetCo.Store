using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WidgetCo.Store.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddApplicationDatabase(context.Configuration);
        services.AddApplicationConfiguration(context.Configuration);
        services.AddApplicationServices();
    })
    .Build();

await host.RunAsync();