using WidgetCo.Store.Core.Enums;
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order order);
        Task<Order> GetOrderAsync(string customerId, string orderId);
        Task<IEnumerable<Order>> GetCustomerOrdersAsync(string customerId);
        Task UpdateOrderStatusAsync(string customerId, string orderId, OrderStatus status);
    }
}
