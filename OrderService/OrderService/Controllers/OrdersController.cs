using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTO;
using OrderService.Application.Commands;
using OrderService.Application.Queries;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("")]
    public class OrdersController(IMediator m) : ControllerBase
    {
        private readonly IMediator _m = m;

        [HttpGet]
        public Task<ActionResult<Guid>> HealthCheck()
            => Task.FromResult<ActionResult<Guid>>(Ok("OrderService is listening..."));

        [HttpPost("/add")]
        public async Task<ActionResult<Guid>> Create(CreateOrderDto dto)
            => Ok(await _m.Send(new CreateOrderCommand(dto)));

        [HttpGet("/get/{id:guid}")]
        public async Task<ActionResult<OrderDto>> Get(Guid id)
            => Ok(await _m.Send(new GetOrderByIdQuery(id)));

        [HttpGet("/get")]
        public async Task<ActionResult<List<OrderDto>>> GetAll()
            => Ok(await _m.Send(new GetOrdersQuery()));

        [HttpPut("/status/{id:guid}")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            await _m.Send(new UpdateOrderStatusCommand(id, status));
            return NoContent();
        }

        [HttpDelete("/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _m.Send(new DeleteOrderCommand(id));
            return NoContent();
        }
    }
}