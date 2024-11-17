using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Storage
{
    public class ProductRepository : IProductRepository
    {
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(
            WidgetCoDbContext context,
            ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == id);
        }
    }
}