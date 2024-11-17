using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record CreateOrderRequest
    {
        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; init; } = default!;

        [Required(ErrorMessage = "At least one item is required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemDto> Items { get; init; } = new();
    }
}
