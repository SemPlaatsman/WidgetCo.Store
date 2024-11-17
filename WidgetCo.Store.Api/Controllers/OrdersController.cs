using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Extensions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Options;
using WidgetCo.Store.Core.Queries;

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
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            request.ValidateAndThrow();
            try
            {
                var command = new InitiateOrderCommand(
                    request.CustomerId,
                    request.Items
                );

                var orderRequestId = await _orderService.InitiateOrderAsync(command);

                return AcceptedAtAction(
                    nameof(GetOrder),
                    new { orderRequestId },
                    new OrderStatusResponse(orderRequestId));
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
                var query = new GetOrderByRequestIdQuery(orderRequestId);
                var order = await _orderService.GetOrderByRequestIdAsync(query);

                return Ok(new OrderStatusResponse(
                    orderRequestId,
                    order));
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
                var command = new ShipOrderCommand(orderId);
                await _orderService.ShipOrderAsync(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, "shipping order");
            }
        }
    }
}