using Microsoft.Extensions.DependencyInjection;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Services;

namespace WidgetCo.Store.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IReviewService, ReviewService>();

            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}