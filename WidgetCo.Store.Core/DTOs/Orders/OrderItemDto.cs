using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record OrderItemDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; init; } = default!;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; init; }
    }
}
