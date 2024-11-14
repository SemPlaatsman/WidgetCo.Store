using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(string productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<string> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
    }
}
