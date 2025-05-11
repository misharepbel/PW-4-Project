using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTO;
using OrderService.Domain.Models;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        public OrdersController(IOrderRepository repo) => _repo = repo;

        [HttpGet("/get/{id}")]
        public async Task<ActionResult<OrderDto>> Get(Guid id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order is null) return NotFound();

            var dto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
            return Ok(dto);
        }

        [HttpPost("/add")]
        public async Task<ActionResult<Guid>> Create(CreateOrderDto dto)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = dto.CustomerName,
                CreatedAt = DateTime.UtcNow,
                Status = "New",
                Items = dto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _repo.AddAsync(order);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order.Id);
        }
    }
}
