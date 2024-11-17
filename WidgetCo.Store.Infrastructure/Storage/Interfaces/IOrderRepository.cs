// Storage related interfaces are kept seperate from the Core interfaces since they are a data concern and not a domain concern. 
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Storage.Interfaces
{
    public interface IOrderRepository
    {
        Task<string> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(string orderId);
        Task<Order?> GetByRequestIdAsync(string requestId);
        Task UpdateAsync(Order order);
    }
}
