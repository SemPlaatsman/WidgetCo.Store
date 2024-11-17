using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.DTOs.Products;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers.Queries
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(
            IProductRepository productRepository,
            ILogger<GetProductByIdQueryHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(query.Id);
                if (product == null) return null;

                return new ProductDto(
                    product.Id,
                    product.Name,
                    product.Price,
                    product.Description,
                    product.ImageUrl
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", query.Id);
                throw new StoreException(
                    "Error retrieving product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }

    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(
            IProductRepository productRepository,
            ILogger<GetAllProductsQueryHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> HandleAsync(GetAllProductsQuery query)
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return products.Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.ImageUrl
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw new StoreException(
                    "Error retrieving products",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}