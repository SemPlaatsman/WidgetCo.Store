namespace WidgetCo.Store.Core.Models
{
    public class OrderRequest
    {
        public string CustomerId { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();
    }
}
