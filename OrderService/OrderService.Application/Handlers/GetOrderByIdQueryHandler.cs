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
        public Task<OrderDto?> Handle(GetOrderByIdQuery q, CancellationToken ct)
            => _srv.GetAsync(q.OrderId);
    }
}
