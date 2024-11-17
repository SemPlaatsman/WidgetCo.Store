using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Data;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Storage.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WidgetCoDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(WidgetCoDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        public async Task<Order?> GetByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByRequestIdAsync(string requestId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderRequestId == requestId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}