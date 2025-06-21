using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Messaging;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace PaymentService.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class PaymentsController(IOrderPaidProducer orderProducer) : ControllerBase
{
    private readonly IOrderPaidProducer _orderProducer = orderProducer;

    [HttpPost("simulate-payment")]
    [SwaggerOperation(Summary = "Simulate payment", Description = "Access: User & Admin")]
    public async Task<IActionResult> SimulatePayment([FromBody] PaymentRequest request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Forbid();

        var orderEvent = new OrderPaidEvent { OrderId = request.OrderId, Email = email };
        await _orderProducer.PublishAsync(orderEvent);

        return Ok();
    }
}

public record PaymentRequest(Guid OrderId);
