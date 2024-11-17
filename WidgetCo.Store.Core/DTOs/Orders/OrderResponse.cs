namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record OrderResponse(
        string Id,
        string CustomerId,
        List<OrderItemDto> Items,
        DateTime CreatedDate,
        DateTime? ShippedDate
    );
}
