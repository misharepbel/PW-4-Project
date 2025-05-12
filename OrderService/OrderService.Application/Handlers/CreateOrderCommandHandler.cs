using MediatR;
using OrderService.Application.Commands;
using OrderService.Application.Services;

namespace OrderService.Application.Handlers
{
    public sealed class CreateOrderCommandHandler
        : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderService _srv;
        public CreateOrderCommandHandler(IOrderService srv) => _srv = srv;
        public Task<Guid> Handle(CreateOrderCommand cmd, CancellationToken ct)
            => _srv.CreateAsync(cmd.Order);
    }
}
