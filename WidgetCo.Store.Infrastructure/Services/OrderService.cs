using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.Commands;
using WidgetCo.Store.Core.Common;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Util;

namespace WidgetCo.Store.Infrastructure.Services
{
    public class OrderService(
        ICommandHandler<InitiateOrderCommand, string> initiateOrderHandler,
        ICommandHandler<CreateOrderCommand, string> createOrderHandler,
        ICommandHandler<ShipOrderCommand, Unit> shipOrderHandler,
        IQueryHandler<GetOrderByRequestIdQuery, OrderResponse?> getByRequestIdHandler,
        IQueryHandler<GetOrderByIdQuery, OrderResponse?> getByIdHandler,
        ILogger<OrderService> logger) : IOrderService
    {
        public Task<string> InitiateOrderAsync(InitiateOrderCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => initiateOrderHandler.HandleAsync(command),
                "Error initiating order for customer {CustomerId}",
                command.CustomerId);

        public Task<string> CreateOrderAsync(CreateOrderCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => createOrderHandler.HandleAsync(command),
                "Error creating order for request {OrderRequestId}",
                command.OrderRequestId);

        public Task ShipOrderAsync(ShipOrderCommand command) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => shipOrderHandler.HandleAsync(command),
                "Error shipping order {OrderId}",
                command.OrderId);

        public Task<OrderResponse?> GetOrderByRequestIdAsync(GetOrderByRequestIdQuery query) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => getByRequestIdHandler.HandleAsync(query),
                "Error retrieving order by request ID {RequestId}",
                query.OrderRequestId);

        public Task<OrderResponse?> GetOrderAsync(GetOrderByIdQuery query) =>
            logger.ExecuteWithExceptionLoggingAsync(
                () => getByIdHandler.HandleAsync(query),
                "Error retrieving order {OrderId}",
                query.OrderId);
    }
}