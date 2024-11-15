using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Infrastructure.Options;
using WidgetCo.Store.Infrastructure.Storage.Entities;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Storage
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TableClient _tableClient;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(
            IOptions<ReviewStorageOptions> options,
            ILogger<ReviewRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = options?.Value?.ConnectionString
                ?? throw new StoreException(
                    "Storage configuration missing",
                    (int)HttpStatusCode.InternalServerError,
                    "Storage connection string was not found in configuration");

            try
            {
                _tableClient = new TableClient(connectionString, options.Value.TableName);
                _tableClient.CreateIfNotExists();
                _logger.LogInformation("Table {TableName} initialized successfully", options.Value.TableName);
            }
            catch (Exception ex)
            {
                var message = $"Failed to initialize table {options.Value.TableName}";
                _logger.LogError(ex, message);
                throw new StoreException(
                    message,
                    (int)HttpStatusCode.InternalServerError,
                    "Could not initialize storage table",
                    ex);
            }
        }

        public async Task<string> CreateAsync(ReviewEntity review)
        {
            await _tableClient.AddEntityAsync(review);
            return review.RowKey;
        }

        public async Task<IEnumerable<ReviewEntity>> GetByProductIdAsync(string productId)
        {
            var reviews = _tableClient.QueryAsync<ReviewEntity>(
                filter: $"PartitionKey eq '{productId}'");

            var results = new List<ReviewEntity>();
            await foreach (var review in reviews)
            {
                results.Add(review);
            }

            return results;
        }
    }
}