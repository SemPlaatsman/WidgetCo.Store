using Microsoft.Extensions.Logging;
using WidgetCo.Store.Core.DTOs.Orders;
using WidgetCo.Store.Core.Interfaces;
using WidgetCo.Store.Core.Queries;
using WidgetCo.Store.Infrastructure.Storage.Interfaces;

namespace WidgetCo.Store.Infrastructure.Handlers
{
    public class GetOrderByRequestIdQueryHandler
        : IQueryHandler<GetOrderByRequestIdQuery, OrderResponse?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByRequestIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResponse?> HandleAsync(GetOrderByRequestIdQuery query)
        {
            var order = await _orderRepository.GetByRequestIdAsync(query.OrderRequestId);
            if (order == null) return null;

            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList(),
                order.CreatedDate,
                order.ShippedDate
            );
        }
    }

    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse?>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderResponse?> HandleAsync(GetOrderByIdQuery query)
        {
            var order = await _orderRepository.GetByIdAsync(query.OrderId);
            if (order == null) return null;

            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList(),
                order.CreatedDate,
                order.ShippedDate
            );
        }
    }
}