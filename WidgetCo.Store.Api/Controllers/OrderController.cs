using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Api.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IOptions<ApiOptions> apiOptions)
            : base(logger, apiOptions)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            try
            {
                var orderId = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder),
                    new { customerId = order.CustomerId, orderId },
                    new { orderId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "creating order");
            }
        }

        [HttpGet("{customerId}/{orderId}")]
        public async Task<IActionResult> GetOrder(string customerId, string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(customerId, orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "retrieving order");
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerOrders(string customerId)
        {
            try
            {
                var orders = await _orderService.GetCustomerOrdersAsync(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "retrieving customer orders");
            }
        }
    }
}
