using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("OrderService is working from Docker!");
        [HttpGet("hello")]
        public IActionResult Hello() => Ok("Root says hello!");
    }
}
