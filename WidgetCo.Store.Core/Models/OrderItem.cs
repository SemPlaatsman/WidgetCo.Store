using System.ComponentModel.DataAnnotations;

namespace WidgetCo.Store.Core.Models
{
    public class OrderItem
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
