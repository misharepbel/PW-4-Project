using MediatR;
using OrderService.Application.Commands;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Handlers
{
    public sealed class UpdateOrderStatusCommandHandler
        : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IOrderRepository _repo;
        public UpdateOrderStatusCommandHandler(IOrderRepository repo) => _repo = repo;

        public async Task Handle(UpdateOrderStatusCommand cmd, CancellationToken ct)
        {
            var order = await _repo.GetByIdAsync(cmd.OrderId);
            if (order is null) throw new InvalidOperationException("Order not found");
            order.Status = cmd.NewStatus;
            await _repo.SaveChangesAsync();
        }
    }
}
