using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Util;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductService(
        IQueryHandler<GetProductByIdQuery, ProductDto?> getProductByIdHandler,
        IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>> getAllProductsHandler,
        ICommandHandler<CreateProductCommand, string> createProductHandler,
        ICommandHandler<UpdateProductCommand, Unit> updateProductHandler,
        ILogger<ProductService> logger) : IProductService
    {
        public Task<ProductDto?> GetProductByIdAsync(string productId) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => getProductByIdHandler.HandleAsync(new GetProductByIdQuery(productId)),
                "Error retrieving product {ProductId}",
                productId);

        public Task<IEnumerable<ProductDto>> GetAllProductsAsync() =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => getAllProductsHandler.HandleAsync(new GetAllProductsQuery()),
                "Error retrieving all products");

        public Task<string> CreateProductAsync(CreateProductCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => createProductHandler.HandleAsync(command),
                "Error creating product {ProductName}",
                command.Name);

        public Task UpdateProductAsync(UpdateProductCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => updateProductHandler.HandleAsync(command),
                "Error updating product {ProductId}",
                command.Id);
    }
}