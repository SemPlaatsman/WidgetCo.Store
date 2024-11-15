using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Products;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(string productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<string> CreateProductAsync(CreateProductCommand product);
        Task UpdateProductAsync(UpdateProductCommand product);
    }
}
