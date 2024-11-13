using Azure.Data.Tables;
using Azure;
using WidgetCo.Store.Core.Enums;

namespace WidgetCo.Store.Infrastructure.Storage
{
    public class OrderEntity : ITableEntity
    {
        public required string PartitionKey { get; set; } // CustomerId
        public required string RowKey { get; set; } // OrderId
        public required string OrderJson { get; set; } = default!; // Serialized order details
        public required OrderStatus Status { get; set; }
        public required DateTime CreatedDate { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
