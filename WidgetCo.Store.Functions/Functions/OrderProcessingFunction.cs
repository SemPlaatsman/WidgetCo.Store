using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Interfaces;
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

        [Function("ProcessOrder")]
        public async Task Run(
            [QueueTrigger("order-processing", Connection = "OrderStorage:ConnectionString")]
            string messageText)
        {
            _logger.LogInformation("Processing message: {MessageText}", messageText);

            OrderProcessingMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<OrderProcessingMessage>(messageText);
                if (message == null)
                {
                    _logger.LogError("Failed to deserialize message");
                    return;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse message");
                return;
            }

            try
            {
                var command = new CreateOrderCommand(
                    message.OrderRequestId,
                    message.CustomerId,
                    message.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList()
                );

                await _orderService.CreateOrderAsync(command);

                _logger.LogInformation(
                    "Successfully created order from request {OrderRequestId}",
                    message.OrderRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to process order request {OrderRequestId}",
                    message.OrderRequestId);
                throw;
            }
        }
    }
}