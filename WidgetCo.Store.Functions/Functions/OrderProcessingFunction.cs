using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Model;
using WidgetCo.Store.Infrastructure.Options;

namespace WidgetCo.Store.Functions.Functions
{
    public class OrderProcessingFunction
    {
        private readonly IOrderService _orderService;
        private readonly OrderStorageOptions _options;
        private readonly ILogger<OrderProcessingFunction> _logger;

        public OrderProcessingFunction(
            IOrderService orderService,
            IOptions<OrderStorageOptions> options,
            ILogger<OrderProcessingFunction> logger)
        {
            _orderService = orderService;
            _options = options.Value;
            _logger = logger;
        }

        // No restful naming for this function because it is triggered by a queue
        [Function("ProcessOrder")]
        public async Task Run([QueueTrigger("order-processing", Connection = "OrderStorage:ConnectionString")] string messageText)
        {
            _logger.LogInformation("Processing message: {MessageText}", messageText);

            OrderProcessingMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<OrderProcessingMessage>(messageText);

                if (message == null)
                {
                    _logger.LogError("Failed to deserialize message to OrderProcessingMessage");
                    return; // Don't retry - message is invalid
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse message: {MessageText}", messageText);
                return; // Don't retry - message is invalid
            }

            try
            {
                // Create order in database
                var order = new Order
                {
                    OrderRequestId = message.OrderRequestId,
                    CustomerId = message.CustomerId,
                    Items = message.Items,
                    CreatedDate = DateTime.UtcNow
                };

                await _orderService.CreateOrderAsync(order);

                _logger.LogInformation(
                    "Successfully created order from request {OrderRequestId}",
                    message.OrderRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to process order request {OrderRequestId} for customer {CustomerId}",
                    message.OrderRequestId,
                    message.CustomerId);
                throw; // Retry the message only for non-deserialization errors
            }
        }
    }
}
