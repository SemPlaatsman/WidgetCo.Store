using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Model
{
    public class OrderProcessingMessage
    {
        public string OrderRequestId { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
