using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.Models
{
    public class OrderRequest
    {
        [Required]
        public string CustomerId { get; set; } = default!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItem> Items { get; set; } = new();
    }
}
