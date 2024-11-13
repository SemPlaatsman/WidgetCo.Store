using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.Models
{
    public class OrderItem
    {
        [Required(ErrorMessage = "ProductId is required")]
        public string ProductId { get; set; } = default!;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
