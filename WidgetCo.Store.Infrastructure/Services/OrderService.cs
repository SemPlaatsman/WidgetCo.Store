using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IRepository<Order> orderRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            var orderId = await _orderRepository.AddAsync(order);

            _logger.LogInformation(
                "Created order {OrderId} for customer {CustomerId}",
                order.OrderId,
                order.CustomerId);

            return orderId;
        }

        public async Task<Order> GetOrderByRequestIdAsync(string orderRequestId)
        {
            // Using the Query() method for custom queries
            var order = await _orderRepository.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);


            if (order == null)
            {
                throw new StoreException(
                    "Order not found",
                    (int)HttpStatusCode.NotFound);
            }

            return order;
        }

        public async Task<Order> GetOrderAsync(string orderId)
        {
            var order = await _orderRepository.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new StoreException(
                    "Order not found",
                    (int)HttpStatusCode.NotFound);
            }

            return order;
        }

        public async Task ShipOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new StoreException(
                    "Order not found",
                    (int)HttpStatusCode.NotFound);
            }

            if (order.ShippedDate.HasValue)
            {
                throw new StoreException(
                    "Order is already shipped",
                    (int)HttpStatusCode.BadRequest);
            }

            order.ShippedDate = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Shipped order {OrderId}", orderId);
        }
    }
}
