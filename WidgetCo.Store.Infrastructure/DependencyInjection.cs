using Microsoft.Extensions.DependencyInjection;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Data.Repository;
using WidgetCo.Store.Infrastructure.Services;

namespace WidgetCo.Store.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IReviewService, ReviewService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IProductService, ProductService>();

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}