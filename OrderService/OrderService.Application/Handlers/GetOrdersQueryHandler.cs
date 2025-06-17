using MediatR;
using OrderService.Application.DTO;
using OrderService.Application.Queries;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Handlers
{
    public sealed class GetOrdersQueryHandler
        : IRequestHandler<GetOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _repo;
        public GetOrdersQueryHandler(IOrderRepository repo) => _repo = repo;

        public async Task<List<OrderDto>> Handle(GetOrdersQuery q, CancellationToken ct)
        {
            var orders = await _repo.GetAllAsync();      // metoda w repo
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                DeliveryLocation = o.DeliveryLocation,
                PaymentMethod = o.PaymentMethod,
                Items = o.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }).ToList();
        }
    }
}
