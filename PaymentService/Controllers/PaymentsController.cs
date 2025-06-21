using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Messaging;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace PaymentService.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class PaymentsController(IOrderPaidProducer orderProducer, IEmailProducer emailProducer) : ControllerBase
{
    private readonly IOrderPaidProducer _orderProducer = orderProducer;
    private readonly IEmailProducer _emailProducer = emailProducer;

    [HttpPost("simulate-payment")]
    [SwaggerOperation(Summary = "Simulate payment", Description = "Access: User & Admin")]
    public async Task<IActionResult> SimulatePayment([FromBody] PaymentRequest request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Forbid();

        var orderEvent = new OrderPaidEvent { OrderId = request.OrderId };
        await _orderProducer.PublishAsync(orderEvent);

        var items = request.Items is null ? string.Empty : string.Join(", ", request.Items);
        var body = $"Payment received for order {request.OrderId} on {DateTime.UtcNow:yyyy-MM-dd}.\n" +
                   $"Amount: {request.Amount:C}\nItems: {items}";
        var message = new EmailMessage(email, "Payment Receipt", body);
        await _emailProducer.PublishAsync(message);

        return Ok();
    }
}

public record PaymentRequest(Guid OrderId, decimal Amount, string[] Items);
