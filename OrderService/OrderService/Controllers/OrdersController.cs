using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OrderService.Application.DTO;
using OrderService.Application.Commands;
using OrderService.Application.Queries;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("")]
    [Authorize]
    public class OrdersController(IMediator m) : ControllerBase
    {
        private readonly IMediator _m = m;

        [HttpGet]
        [AllowAnonymous]
        public Task<ActionResult<Guid>> HealthCheck()
            => Task.FromResult<ActionResult<Guid>>(Ok("OrderService is listening..."));

        [HttpGet("/get/{id:guid}")]
        public async Task<ActionResult<OrderDto?>> Get(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            var order = await _m.Send(new GetOrderByIdQuery(id, userId, isAdmin));
            if (order is null)
                return Forbid();
            return Ok(order);
        }

        [HttpGet("/get")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
            => Ok(await _m.Send(new GetOrdersQuery()));

        [HttpGet("/my")]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _m.Send(new GetOrdersByUserIdQuery(userId));
            return Ok(orders);
        }

        [HttpPut("/status/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            await _m.Send(new UpdateOrderStatusCommand(id, status));
            return NoContent();
        }

        [HttpDelete("/delete/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _m.Send(new DeleteOrderCommand(id));
            return NoContent();
        }
    }
}
