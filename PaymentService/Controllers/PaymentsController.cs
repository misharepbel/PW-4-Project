using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Messaging;
using PaymentService.Cache;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace PaymentService.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class PaymentsController(IOrderPaidProducer orderProducer, IOrderCache cache) : ControllerBase
{
    private readonly IOrderPaidProducer _orderProducer = orderProducer;
    private readonly IOrderCache _cache = cache;

    [HttpGet("cached")]
    [SwaggerOperation(Summary = "Show cached orders", Description = "Access: User & Admin")]
    public IActionResult GetCachedOrders()
        => Ok(_cache.Orders);

    [HttpPost("simulate-payment")]
    [SwaggerOperation(Summary = "Simulate payment", Description = "Access: User & Admin")]
    public async Task<IActionResult> SimulatePayment([FromBody] PaymentRequest request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Forbid();

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
            return Forbid();

        var userId = Guid.Parse(userIdStr);

        if (!_cache.Contains(request.OrderId))
            return NotFound();

        var ownerId = _cache.GetUserId(request.OrderId);
        if (ownerId != userId)
            return Forbid();

        var orderEvent = new OrderPaidEvent
        {
            OrderId = request.OrderId,
            UserId = userId,
            Email = email
        };
        await _orderProducer.PublishAsync(orderEvent);

        return Ok();
    }
}

public record PaymentRequest(Guid OrderId);
