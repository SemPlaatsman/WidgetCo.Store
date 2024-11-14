using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order order);
        Task<Order> GetOrderByRequestIdAsync(string orderRequestId);
        Task<Order> GetOrderAsync(string orderId);
        Task ShipOrderAsync(string orderId);
    }
}
