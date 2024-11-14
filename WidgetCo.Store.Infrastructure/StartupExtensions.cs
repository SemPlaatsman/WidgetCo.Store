using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Infrastructure.Data;
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

            services.Configure<BlobStorageOptions>(
                configuration.GetSection(BlobStorageOptions.SectionName));

            return services;
        }

        public static IServiceCollection AddApplicationDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<WidgetCoDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(WidgetCoDbContext).Assembly.FullName)));

            return services;
        }
    }
}
