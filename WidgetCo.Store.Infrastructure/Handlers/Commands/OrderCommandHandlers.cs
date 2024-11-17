using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.Exceptions;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Models;
using WidgetCo.Store.Infrastructure.Model;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers
{
    public class InitiateOrderCommandHandler : ICommandHandler<InitiateOrderCommand, string>
    {
        private readonly IProductService _productService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly ILogger<InitiateOrderCommandHandler> _logger;

        public InitiateOrderCommandHandler(
            IProductService productService,
            IOrderMessageService orderMessageService,
            ILogger<InitiateOrderCommandHandler> logger)
        {
            _productService = productService;
            _orderMessageService = orderMessageService;
            _logger = logger;
        }

        public async Task<string> HandleAsync(InitiateOrderCommand command)
        {
            // Validate products exist
            foreach (var item in command.Items)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new StoreException(
                        $"Product {item.ProductId} not found",
                        (int)HttpStatusCode.BadRequest);
                }
            }

            var message = new OrderProcessingMessage
            {
                CustomerId = command.CustomerId,
                Items = command.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _orderMessageService.SendOrderProcessingMessageAsync(
                JsonSerializer.Serialize(message));

            _logger.LogInformation(
                "Initiated order request {OrderRequestId} for customer {CustomerId}",
                message.OrderRequestId,
                command.CustomerId);

            return message.OrderRequestId;
        }
    }

    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, string>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<string> HandleAsync(CreateOrderCommand command)
        {
            var order = new Order
            {
                OrderRequestId = command.OrderRequestId,
                CustomerId = command.CustomerId,
                Items = command.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                CreatedDate = DateTime.UtcNow
            };

            var orderId = await _orderRepository.CreateAsync(order);

            _logger.LogInformation(
                "Created order {OrderId} for customer {CustomerId}",
                orderId,
                command.CustomerId);

            return orderId;
        }
    }

    public class ShipOrderCommandHandler : ICommandHandler<ShipOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ShipOrderCommandHandler> _logger;

        public ShipOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<ShipOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> HandleAsync(ShipOrderCommand command)
        {
            var order = await _orderRepository.GetByIdAsync(command.OrderId);

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

            _logger.LogInformation("Shipped order {OrderId}", command.OrderId);

            return Unit.Value;
        }
    }
}