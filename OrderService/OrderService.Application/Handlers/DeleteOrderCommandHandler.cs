using MediatR;
using OrderService.Application.Commands;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Handlers
{
    public sealed class DeleteOrderCommandHandler
        : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IOrderRepository _repo;
        public DeleteOrderCommandHandler(IOrderRepository repo) => _repo = repo;

        public async Task Handle(DeleteOrderCommand cmd, CancellationToken ct)
        {
            await _repo.DeleteAsync(cmd.OrderId);
        }
    }
}
