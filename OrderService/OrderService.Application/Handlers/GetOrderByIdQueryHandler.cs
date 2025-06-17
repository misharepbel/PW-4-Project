using MediatR;
using OrderService.Application.DTO;
using OrderService.Application.Queries;
using OrderService.Application.Services;

namespace OrderService.Application.Handlers
{
    public sealed class GetOrderByIdQueryHandler
        : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderService _srv;
        public GetOrderByIdQueryHandler(IOrderService srv) => _srv = srv;

        public async Task<OrderDto?> Handle(GetOrderByIdQuery q, CancellationToken ct)
        {
            var order = await _srv.GetAsync(q.OrderId, ct);
            if (order is null)
                return null;

            if (order.UserId != q.UserId && !q.IsAdmin)
                return null;

            return order;
        }
    }
}
