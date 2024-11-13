using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Infrastructure.Options;

namespace WidgetCo.Store.Infrastructure
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddApplicationConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiOptions>(
                configuration.GetSection(ApiOptions.SectionName));

            services.Configure<StorageOptions>(
                configuration.GetSection(StorageOptions.SectionName));

            return services;
        }
    }
}
