using Azure.Data.Tables;
using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WidgetCo.Store.Core.Enums;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Options;
using WidgetCo.Store.Infrastructure.Storage;
using Azure.Storage.Queues;
using WidgetCo.Store.Core.Extensions;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly TableClient _tableClient;
        private readonly QueueClient _queueClient;
        private readonly ILogger<OrderService> _logger;
        private const string TableName = "Orders";
        private const string QueueName = "order-processing";

        public OrderService(
            IOptions<StorageOptions> options,
            ILogger<OrderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var connectionString = options?.Value?.ConnectionString
                ?? throw new StoreException(
                    "Storage configuration missing",
                    (int)HttpStatusCode.InternalServerError);

            try
            {
                _tableClient = new TableClient(connectionString, TableName);
                _tableClient.CreateIfNotExists();

                _queueClient = new QueueClient(connectionString, QueueName);
                _queueClient.CreateIfNotExists();

                _logger.LogInformation("Order service initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize order service");
                throw new StoreException(
                    "Failed to initialize order service",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            try
            {
                if (order == null)
                    throw new StoreException(
                        "Order data is required",
                        (int)HttpStatusCode.BadRequest);

                order.ValidateAndThrow();
                order.CreatedDate = DateTime.UtcNow;

                var orderEntity = new OrderEntity
                {
                    PartitionKey = order.CustomerId,
                    RowKey = order.OrderId,
                    OrderJson = JsonSerializer.Serialize(order),
                    Status = OrderStatus.Pending,
                    CreatedDate = order.CreatedDate
                };

                await _tableClient.AddEntityAsync(orderEntity);

                // Add to processing queue
                await _queueClient.SendMessageAsync(
                    JsonSerializer.Serialize(new { order.CustomerId, order.OrderId }));

                _logger.LogInformation(
                    "Created order {OrderId} for customer {CustomerId}",
                    order.OrderId,
                    order.CustomerId);

                return order.OrderId;
            }
            catch (StoreException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                throw new StoreException(
                    "Failed to create order",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<Order> GetOrderAsync(string customerId, string orderId)
        {
            try
            {
                var orderEntity = await _tableClient.GetEntityAsync<OrderEntity>(
                    customerId,
                    orderId);

                return JsonSerializer.Deserialize<Order>(orderEntity.Value.OrderJson)
                    ?? throw new StoreException(
                        "Order not found",
                        (int)HttpStatusCode.NotFound);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                throw new StoreException(
                    "Order not found",
                    (int)HttpStatusCode.NotFound);
            }
            catch (StoreException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve order");
                throw new StoreException(
                    "Failed to retrieve order",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task<IEnumerable<Order>> GetCustomerOrdersAsync(string customerId)
        {
            try
            {
                var orders = new List<Order>();
                var queryResults = _tableClient.QueryAsync<OrderEntity>(
                    filter: $"PartitionKey eq '{customerId}'");

                await foreach (var entity in queryResults)
                {
                    var order = JsonSerializer.Deserialize<Order>(entity.OrderJson);
                    if (order != null)
                    {
                        orders.Add(order);
                    }
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve customer orders");
                throw new StoreException(
                    "Failed to retrieve customer orders",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }

        public async Task UpdateOrderStatusAsync(string customerId, string orderId, OrderStatus status)
        {
            try
            {
                var orderEntity = await _tableClient.GetEntityAsync<OrderEntity>(customerId, orderId);
                var order = JsonSerializer.Deserialize<Order>(orderEntity.Value.OrderJson);
                if (order == null)
                {
                    throw new StoreException(
                        "Order not found",
                        (int)HttpStatusCode.NotFound);
                }

                order.Status = status;
                orderEntity.Value.OrderJson = JsonSerializer.Serialize(order);
                orderEntity.Value.Status = status;

                await _tableClient.UpdateEntityAsync(orderEntity.Value, orderEntity.Value.ETag);

                _logger.LogInformation(
                    "Updated order {OrderId} status to {Status}",
                    orderId,
                    status);
            }
            catch (StoreException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order status");
                throw new StoreException(
                    "Failed to update order status",
                    (int)HttpStatusCode.InternalServerError,
                    ex);
            }
        }
    }
}
