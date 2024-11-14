using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Model;

namespace WidgetCo.Store.Functions.Functions
{
    public class OrderProcessingFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderProcessingFunction> _logger;

        public OrderProcessingFunction(
            IOrderService orderService,
            ILogger<OrderProcessingFunction> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // No restful naming for this function because it is triggered by a queue
        [Function("ProcessOrder")]
        public async Task Run(
            [QueueTrigger("order-processing")] string messageText)
        {
            try
            {
                var message = JsonSerializer.Deserialize<OrderProcessingMessage>(messageText);

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
                _logger.LogError(ex, "Failed to process order request");
                throw; // Retry the message
            }
        }
    }
}
