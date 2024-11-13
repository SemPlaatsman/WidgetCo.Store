using Azure.Data.Tables;
using Azure;

namespace WidgetCo.Store.Infrastructure.Storage
{
    public class ReviewEntity : ITableEntity
    {
        public required string PartitionKey { get; set; } // ProductId
        public required string RowKey { get; set; } // Unique identifier for the review
        public required string ReviewText { get; set; }
        public required int Rating { get; set; }
        public required DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
