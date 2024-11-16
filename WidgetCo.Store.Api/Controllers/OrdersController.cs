using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Core.Options;

namespace WidgetCo.Store.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger,
            IOptions<ApiOptions> apiOptions)
            : base(logger, apiOptions)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest request)
        {
            try
            {
                var orderRequestId = await _orderService.InitiateOrderAsync(request);

                return AcceptedAtAction(
                    nameof(GetOrder),
                    new { orderRequestId },
                    new { requestId = orderRequestId });
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
                        new { requestId = orderRequestId });
                }

                return Ok(new
                {
                    order.Id,
                    order.CustomerId,
                    order.Items,
                    order.CreatedDate,
                    order.ShippedDate
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
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "shipping order");
            }
        }
    }
}