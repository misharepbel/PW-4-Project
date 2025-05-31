using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[Route("")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("CatalogService is healthy.");
}
