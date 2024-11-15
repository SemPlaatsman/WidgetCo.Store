using Azure.Data.Tables;
using Azure;
using WidgetCo.Store.Core.Models;

namespace WidgetCo.Store.Infrastructure.Storage.Entities
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

        public static ReviewEntity FromDomain(Review review)
        {
            return new ReviewEntity
            {
                PartitionKey = review.ProductId,
                RowKey = review.Id,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                CreatedDate = review.CreatedDate
            };
        }

        public Review ToDomain()
        {
            return new Review
            {
                Id = RowKey,
                ProductId = PartitionKey,
                ReviewText = ReviewText,
                Rating = Rating,
                CreatedDate = CreatedDate
            };
        }
    }
}
