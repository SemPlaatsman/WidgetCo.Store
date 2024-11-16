using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(Order order);
        Task<string> InitiateOrderAsync(OrderRequest request);
        Task<Order?> GetOrderByRequestIdAsync(string orderRequestId);
        Task<Order> GetOrderAsync(string orderId);
        Task ShipOrderAsync(string orderId);
    }
}