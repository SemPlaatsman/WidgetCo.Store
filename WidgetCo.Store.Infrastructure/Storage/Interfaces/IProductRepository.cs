// Storage related interfaces are kept seperate from the Core interfaces since they are a data concern and not a domain concern. 
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Storage.Interfaces
{
    public interface IProductRepository
    {
        Task<string> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task UpdateAsync(Product product);
        Task<bool> ExistsAsync(string id);
    }
}
