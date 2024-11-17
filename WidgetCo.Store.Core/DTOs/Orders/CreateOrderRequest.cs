namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record CreateOrderRequest(
        string CustomerId,
        List<OrderItemDto> Items
    );
}
