using Microsoft.Extensions.DependencyInjection;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.DTOs.Reviews;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Data.Repository;
using WidgetCo.Store.Infrastructure.Handlers.Commands;
using WidgetCo.Store.Infrastructure.Handlers.Queries;
using WidgetCo.Store.Infrastructure.Services;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;
using WidgetCo.Store.Infrastructure.Storage;
using WidgetCo.Store.Infrastructure.Handlers;
using WidgetCo.Store.Core.Interfaces.Storage;

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
            services.AddScoped<IProductImageService, ProductImageService>();

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();

            // Register CQRS
            services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductDto?>, GetProductByIdQueryHandler>();
            services.AddScoped<IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>, GetAllProductsQueryHandler>();
            services.AddScoped<ICommandHandler<CreateProductCommand, string>, CreateProductCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateProductCommand, Unit>, UpdateProductCommandHandler>();

            services.AddScoped<ICommandHandler<CreateReviewCommand, string>, CreateReviewCommandHandler>();
            services.AddScoped<IQueryHandler<GetProductReviewsQuery, IEnumerable<ReviewDto>>, GetProductReviewsQueryHandler>();

            return services;
        }
    }
}