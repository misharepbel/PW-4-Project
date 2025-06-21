using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PaymentService.Controllers;

[Route("")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Health check", Description = "Access: Public")]
    public IActionResult Get() => Ok("PaymentService is listening...");
}
