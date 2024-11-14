using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Infrastructure.Model;

namespace WidgetCo.Store.Api.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly QueueClient _queueClient;
        private readonly IOrderService _orderService;

        public OrdersController(
            IProductService productService,
            QueueServiceClient queueServiceClient,
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IOptions<ApiOptions> apiOptions)
            : base(logger, apiOptions)
        {
            _productService = productService;
            _orderService = orderService;
            _queueClient = queueServiceClient.GetQueueClient("order-processing");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest request)
        {
            try
            {
                // Validate products exist
                foreach (var item in request.Items)
                {
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new StoreException(
                            $"Product {item.ProductId} not found",
                            (int)HttpStatusCode.BadRequest);
                    }
                }

                // Create queue message
                var message = new OrderProcessingMessage
                {
                    CustomerId = request.CustomerId,
                    Items = request.Items
                };

                // Add to queue
                await _queueClient.SendMessageAsync(JsonSerializer.Serialize(message));

                return AcceptedAtAction(
                    nameof(GetOrder),
                    new { orderRequestId = message.OrderRequestId },
                    new
                    {
                        message = "Order request accepted",
                        trackingId = message.OrderRequestId
                    });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "submitting order");
            }
        }

        [HttpGet("{orderRequestId}")]
        public async Task<IActionResult> GetOrder(string orderRequestId)
        {
            try
            {
                var order = await _orderService.GetOrderByRequestIdAsync(orderRequestId);
                if (order == null)
                {
                    return AcceptedAtAction(
                        nameof(GetOrder),
                        new { orderRequestId },
                        new { message = "Order is being processed" });
                }

                return Ok(new
                {
                    order.OrderId,
                    order.CustomerId,
                    order.Items,
                    order.CreatedDate,
                    order.ShippedDate,
                    Status = GetOrderStatus(order)
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "retrieving order");
            }
        }

        [HttpPost("{orderId}/ship")]
        public async Task<IActionResult> ShipOrder(string orderId)
        {
            try
            {
                await _orderService.ShipOrderAsync(orderId);
                return Ok(new { message = "Order marked as shipped" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "shipping order");
            }
        }

        private static string GetOrderStatus(Order order)
        {
            return order.ShippedDate != null ? "Shipped" : "Created";
        }
    }
}
