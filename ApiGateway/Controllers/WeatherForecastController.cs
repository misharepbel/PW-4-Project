using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger = logger;

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("TestOrderServiceResponse")]
        public async Task<IActionResult> CallOrderService([FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("OrderService");
            var response = await client.GetAsync("");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to call OrderService");

            var content = await response.Content.ReadAsStringAsync();
            return Ok($"OrderService responded: {content}");
        }
    }
}
