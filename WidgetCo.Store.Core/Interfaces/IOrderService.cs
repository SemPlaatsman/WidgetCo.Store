using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Queries;

namespace WidgetCo.Store.Core.Interfaces
{
    public interface IOrderService
    {
        Task<string> InitiateOrderAsync(InitiateOrderCommand command);
        Task<string> CreateOrderAsync(CreateOrderCommand command);
        Task ShipOrderAsync(ShipOrderCommand command);
        Task<OrderResponse?> GetOrderByRequestIdAsync(GetOrderByRequestIdQuery query);
        Task<OrderResponse?> GetOrderAsync(GetOrderByIdQuery query);
    }
}