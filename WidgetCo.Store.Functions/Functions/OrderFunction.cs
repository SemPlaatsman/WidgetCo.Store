using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Functions.Functions
{
    public class OrderFunction
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderFunction> _logger;
        private readonly ApiOptions _apiOptions;

        public OrderFunction(
            IOrderService orderService,
            ILogger<OrderFunction> logger,
            IOptions<ApiOptions> apiOptions)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiOptions = apiOptions.Value;
        }

        [Function("CreateOrder")]
        public async Task<IActionResult> CreateOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var order = JsonSerializer.Deserialize<Order>(
                    requestBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var orderId = await _orderService.CreateOrderAsync(order);
                return new CreatedResult(string.Empty, new { orderId });
            }
            catch (StoreException ex)
            {
                _logger.LogWarning(ex, "Store exception occurred while creating order");
                var error = new
                {
                    message = ex.Message,
                    details = _apiOptions.ReturnDetailedErrors ?
                        ex.DetailedMessage ?? ex.OriginalException?.Message : null
                };
                return new ObjectResult(error) { StatusCode = ex.StatusCode };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating order");
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred",
                    details = _apiOptions.ReturnDetailedErrors ? ex.Message : null
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        [Function("GetOrder")]
        public async Task<IActionResult> GetOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{customerId}/{orderId}")]
        HttpRequest req,
            string customerId,
            string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(customerId, orderId);
                return new OkObjectResult(order);
            }
            catch (StoreException ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating order");
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred",
                    details = _apiOptions.ReturnDetailedErrors ? ex.Message : null
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        [Function("GetCustomerOrders")]
        public async Task<IActionResult> GetCustomerOrders(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{customerId}")]
        HttpRequest req,
            string customerId)
        {
            try
            {
                var orders = await _orderService.GetCustomerOrdersAsync(customerId);
                return new OkObjectResult(orders);
            }
            catch (StoreException ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating order");
                return new ObjectResult(new
                {
                    message = "An unexpected error occurred",
                    details = _apiOptions.ReturnDetailedErrors ? ex.Message : null
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
