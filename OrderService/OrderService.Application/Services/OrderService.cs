// OrderService.Application/Services/OrderService.cs
using OrderService.Application.DTO;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using System.Linq;

namespace OrderService.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IOrderCreatedProducer _producer;

        public OrderService(IOrderRepository repo, IOrderCreatedProducer producer)
        {
            _repo = repo;
            _producer = producer;
        }

        public async Task<Guid> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            if (dto.Items.Count == 0)
                throw new InvalidOperationException("Order must contain at least one item.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                OrderDate = DateTime.UtcNow,
                Status = "New",
                OrderItems = []
            };

            foreach (var i in dto.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                });
            }

            await _repo.AddAsync(order);

            var evt = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
            await _producer.PublishAsync(evt, ct);

            return order.Id;
        }

        public async Task<OrderDto?> GetAsync(Guid id, CancellationToken ct = default)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order is null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
