namespace WidgetCo.Store.Core.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrderRequestId { get; set; } = default!;
        public string CustomerId { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
    }
}
