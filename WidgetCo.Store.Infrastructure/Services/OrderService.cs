using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICommandHandler<InitiateOrderCommand, string> _initiateOrderHandler;
        private readonly ICommandHandler<CreateOrderCommand, string> _createOrderHandler;
        private readonly ICommandHandler<ShipOrderCommand, Unit> _shipOrderHandler;
        private readonly IQueryHandler<GetOrderByRequestIdQuery, OrderResponse?> _getByRequestIdHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, OrderResponse?> _getByIdHandler;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ICommandHandler<InitiateOrderCommand, string> initiateOrderHandler,
            ICommandHandler<CreateOrderCommand, string> createOrderHandler,
            ICommandHandler<ShipOrderCommand, Unit> shipOrderHandler,
            IQueryHandler<GetOrderByRequestIdQuery, OrderResponse?> getByRequestIdHandler,
            IQueryHandler<GetOrderByIdQuery, OrderResponse?> getByIdHandler,
            ILogger<OrderService> logger)
        {
            _initiateOrderHandler = initiateOrderHandler;
            _createOrderHandler = createOrderHandler;
            _shipOrderHandler = shipOrderHandler;
            _getByRequestIdHandler = getByRequestIdHandler;
            _getByIdHandler = getByIdHandler;
            _logger = logger;
        }

        public async Task<string> InitiateOrderAsync(InitiateOrderCommand command)
        {
            try
            {
                return await _initiateOrderHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating order for customer {CustomerId}", command.CustomerId);
                throw;
            }
        }

        public async Task<string> CreateOrderAsync(CreateOrderCommand command)
        {
            try
            {
                return await _createOrderHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for request {OrderRequestId}", command.OrderRequestId);
                throw;
            }
        }

        public async Task ShipOrderAsync(ShipOrderCommand command)
        {
            try
            {
                await _shipOrderHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shipping order {OrderId}", command.OrderId);
                throw;
            }
        }

        public async Task<OrderResponse?> GetOrderByRequestIdAsync(GetOrderByRequestIdQuery query)
        {
            try
            {
                return await _getByRequestIdHandler.HandleAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order by request ID {RequestId}", query.OrderRequestId);
                throw;
            }
        }

        public async Task<OrderResponse?> GetOrderAsync(GetOrderByIdQuery query)
        {
            try
            {
                return await _getByIdHandler.HandleAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", query.OrderId);
                throw;
            }
        }
    }
}