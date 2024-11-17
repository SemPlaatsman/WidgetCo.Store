namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record OrderItemDto(
        string ProductId,
        int Quantity
    );
}
