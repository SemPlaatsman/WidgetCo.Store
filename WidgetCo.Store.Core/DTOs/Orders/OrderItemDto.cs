using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.DTOs.Orders
{
    public record OrderItemDto(
        [Required(ErrorMessage = "ProductId is required")]
        string ProductId,
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity);
}
