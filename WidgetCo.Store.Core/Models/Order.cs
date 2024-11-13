using System.ComponentModel.DataAnnotations;
using WidgetCo.Store.Core.Enums;

namespace WidgetCo.Store.Core.Models
{
    public class Order
    {
        [Required(ErrorMessage = "OrderId is required")]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; set; } = default!;

        [Required(ErrorMessage = "Items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItem> Items { get; set; } = new();

        public DateTime CreatedDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
