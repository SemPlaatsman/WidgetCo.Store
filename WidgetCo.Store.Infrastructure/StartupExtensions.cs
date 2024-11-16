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

            services.Configure<ReviewStorageOptions>(
                configuration.GetSection(ReviewStorageOptions.SectionName));

            services.Configure<ProductImageStorageOptions>(
                configuration.GetSection(ProductImageStorageOptions.SectionName));

            services.Configure<OrderStorageOptions>(
                configuration.GetSection(OrderStorageOptions.SectionName));

            return services;
        }

        public static IServiceCollection AddApplicationDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<WidgetCoDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlSeverConnectionString"),
                    b => b.MigrationsAssembly(typeof(WidgetCoDbContext).Assembly.FullName)));

            return services;
        }
    }
}
