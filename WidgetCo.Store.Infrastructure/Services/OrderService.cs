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
        private readonly WidgetCoDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            WidgetCoDbContext dbContext,
            ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Created order {OrderId} for customer {CustomerId}",
                order.OrderId,
                order.CustomerId);

            return order.OrderId;
        }

        public async Task<Order> GetOrderByRequestIdAsync(string orderRequestId)
        {
            return await _dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);
        }

        public async Task<Order> GetOrderAsync(string orderId)
        {
            var order = await _dbContext.Orders
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
            var order = await _dbContext.Orders.FindAsync(orderId);

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
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Shipped order {OrderId}", orderId);
        }
    }
}
