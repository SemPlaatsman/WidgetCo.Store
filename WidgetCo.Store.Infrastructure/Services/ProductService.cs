using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly WidgetCoDbContext _dbContext;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            WidgetCoDbContext dbContext,
            ILogger<ProductService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Product?> GetProductByIdAsync(string productId)
        {
            try
            {
                return await _dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductId == productId);
            }
            catch (Exception ex)
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
                return await _dbContext.Products
                    .AsNoTracking()
                    .ToListAsync();
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

        public async Task<string> CreateProductAsync(Product product)
        {
            try
            {
                product.ValidateAndThrow();

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Created product {ProductId}", product.ProductId);

                return product.ProductId;
            }
            catch (Exception ex)
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
                product.ValidateAndThrow();

                var existingProduct = await _dbContext.Products.FindAsync(product.ProductId);

                if (existingProduct == null)
                {
                    throw new StoreException(
                        "Product not found",
                        (int)HttpStatusCode.NotFound);
                }

                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.ImageUrl = product.ImageUrl;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated product {ProductId}", product.ProductId);
            }
            catch (StoreException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", product.ProductId);
                throw new StoreException(
                    "Error updating product",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}
