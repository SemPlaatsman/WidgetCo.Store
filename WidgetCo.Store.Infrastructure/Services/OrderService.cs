using Microsoft.Extensions.Logging;
using System.Net;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Queues;
using System.Text.Json;
using WidgetCo.Store.Infrastructure.Model;
using Microsoft.Extensions.Options;
using WidgetCo.Store.Infrastructure.Options;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IProductService _productService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IRepository<Order> orderRepository,
            IProductService productService,
            IOrderMessageService orderMessageService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _orderMessageService = orderMessageService;
            _logger = logger;
        }

        public async Task<string> CreateOrderAsync(Order order)
        {
            var orderId = await _orderRepository.AddAsync(order);

            _logger.LogInformation(
                "Created order {OrderId} for customer {CustomerId}",
                order.Id,
                order.CustomerId);

            return orderId;
        }

        public async Task<string> InitiateOrderAsync(OrderRequest request)
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
            await _orderMessageService.SendOrderProcessingMessageAsync(JsonSerializer.Serialize(message));

            _logger.LogInformation(
                "Initiated order request {OrderRequestId} for customer {CustomerId}",
                message.OrderRequestId,
                request.CustomerId);

            return message.OrderRequestId;
        }

        public async Task<Order?> GetOrderByRequestIdAsync(string orderRequestId)
        {
            return await _orderRepository.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderRequestId == orderRequestId);
        }

        public async Task<Order> GetOrderAsync(string orderId)
        {
            var order = await _orderRepository.Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

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
