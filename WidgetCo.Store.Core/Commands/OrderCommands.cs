using WidgetCo.Store.Core.DTOs.Orders;

namespace WidgetCo.Store.Core.Commands
{
    public record InitiateOrderCommand(
        string CustomerId,
        List<OrderItemDto> Items
    );

    public record CreateOrderCommand(
        string OrderRequestId,
        string CustomerId,
        List<OrderItemDto> Items
    );

    public record ShipOrderCommand(string OrderId);
}
