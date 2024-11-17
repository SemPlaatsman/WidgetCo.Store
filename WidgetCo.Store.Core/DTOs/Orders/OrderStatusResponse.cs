namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record OrderStatusResponse(
        string RequestId,
        OrderResponse? Order = null
    );
}
