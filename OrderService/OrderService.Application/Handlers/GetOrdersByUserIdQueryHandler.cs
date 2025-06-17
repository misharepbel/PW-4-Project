using MediatR;
using OrderService.Application.DTO;
using OrderService.Application.Queries;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Handlers
{
    public sealed class GetOrdersByUserIdQueryHandler
        : IRequestHandler<GetOrdersByUserIdQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _repo;
        public GetOrdersByUserIdQueryHandler(IOrderRepository repo) => _repo = repo;

        public async Task<List<OrderDto>> Handle(GetOrdersByUserIdQuery query, CancellationToken ct)
        {
            var orders = await _repo.GetByUserIdAsync(query.UserId);
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Items = o.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }
    }
}
