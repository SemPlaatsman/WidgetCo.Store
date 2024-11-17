using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Infrastructure.Model;

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
        string messageText,
        FunctionContext context)
    {
        _logger.LogInformation("Processing message: {MessageText}", messageText);

        OrderProcessingMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<OrderProcessingMessage>(messageText);
            if (message == null)
            {
                _logger.LogError("Failed to deserialize message: {MessageText}", messageText);
                // Let the message be deleted from the queue since it's invalid
                return;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse message: {MessageText}", messageText);
            // Let the message be deleted from the queue since it's invalid
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
        catch (StoreException ex)
        {
            _logger.LogWarning(ex,
                "Business logic error processing order request {OrderRequestId}: {Message}",
                message.OrderRequestId,
                ex.Message);

            // For business logic errors, we might want to move the message to a dead letter queue
            // or store it in an error table for manual review
            throw; // This will cause the message to be retried according to the queue's retry policy
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error processing order request {OrderRequestId}",
                message.OrderRequestId);

            // For unexpected errors, we definitely want to retry
            throw;
        }
    }
}