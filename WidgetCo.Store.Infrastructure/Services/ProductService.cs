using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IRepository<Product> productRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Product?> GetProductByIdAsync(string productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new StoreException(
                        $"Product with ID {productId} not found",
                        (int)HttpStatusCode.NotFound);
                }
                return product;
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId}", productId);
                throw new StoreException(
                    "Error retrieving product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw new StoreException(
                    "Error retrieving products",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<string> CreateProductAsync(Product product)
        {
            try
            {
                product.ValidateAndThrow();

                return await _productRepository.AddAsync(product);
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error creating product");
                throw new StoreException(
                    "Error creating product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                // Verify product exists
                if (await _productRepository.GetByIdAsync(product.Id) == null)
                {
                    throw new StoreException(
                        $"Product with ID {product.Id} not found",
                        (int)HttpStatusCode.NotFound);
                }

                product.ValidateAndThrow();
                await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Updated product {ProductId}", product.Id);
            }
            catch (Exception ex) when (ex is not StoreException)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", product.Id);
                throw new StoreException(
                    "Error updating product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}
