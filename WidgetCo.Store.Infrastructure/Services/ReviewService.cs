using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Options;
using WidgetCo.Store.Infrastructure.Storage;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<ReviewService> _logger;
        private const string TableName = "ProductReviews";

        public ReviewService(
            IOptions<ReviewStorageOptions> options,
            ILogger<ReviewService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = options?.Value?.ConnectionString
                ?? throw new StoreException(
                    "Storage configuration missing",
                    (int)HttpStatusCode.InternalServerError,
                    "Storage connection string was not found in configuration");

            try
            {
                _tableClient = new TableClient(connectionString, TableName);
                _tableClient.CreateIfNotExists();
                _logger.LogInformation("Table {TableName} initialized successfully", TableName);
            }
            catch (Exception ex)
            {
                var message = $"Failed to initialize table {TableName}";
                _logger.LogError(ex, message);
                throw new StoreException(
                    message,
                    (int)HttpStatusCode.InternalServerError,
                    "Could not initialize storage table",
                    ex);
            }
        }

        public async Task AddReviewAsync(Review review)
        {
            try
            {
                if (review == null)
                    throw new StoreException(
                        "Review data is required",
                        (int)HttpStatusCode.BadRequest);

                review.ValidateAndThrow();

                var reviewEntity = new ReviewEntity
                {
                    PartitionKey = review.ProductId,
                    RowKey = Guid.NewGuid().ToString(),
                    ReviewText = review.ReviewText,
                    Rating = review.Rating,
                    CreatedDate = DateTime.UtcNow
                };

                await _tableClient.AddEntityAsync(reviewEntity);
                _logger.LogInformation("Added review for product {ProductId}", review.ProductId);
            }
            catch (StoreException) { throw; }
            catch (Exception ex)
            {
                var message = $"Failed to add review for product {review?.ProductId}";
                _logger.LogError(ex, message);
                throw new StoreException(
                    message,
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred while saving the review",
                    ex);
            }
        }

        public async Task<IEnumerable<Review>> GetProductReviewsAsync(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                    throw new StoreException(
                        "Product ID is required",
                        (int)HttpStatusCode.BadRequest);

                var reviews = _tableClient.QueryAsync<ReviewEntity>(
                    filter: $"PartitionKey eq '{productId}'");

                var results = new List<Review>();
                await foreach (var review in reviews)
                {
                    results.Add(new Review
                    {
                        ProductId = review.PartitionKey,
                        ReviewText = review.ReviewText,
                        Rating = review.Rating
                    });
                }

                _logger.LogInformation("Retrieved {Count} reviews for product {ProductId}",
                    results.Count, productId);

                return results;
            }
            catch (StoreException){ throw; }
            catch (Exception ex)
            {
                var message = $"Failed to retrieve reviews for product {productId}";
                _logger.LogError(ex, message);
                throw new StoreException(
                    message,
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred while retrieving reviews",
                    ex);
            }
        }
    }
}
