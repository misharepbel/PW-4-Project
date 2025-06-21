using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger.Extensions;
using System.Diagnostics;
using System.Xml.Linq;


namespace ApiGateway
{
    public class Program
    {
        // a method used to wait for microservices to start listening
        private static async Task WaitForService(string name, string url, int maxRetries = 10, int delaySeconds = 2)
        {
            using var httpClient = new HttpClient();
            Console.WriteLine($"Waiting for: {name} {url}");
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"{name} is available.");
                        return;
                    }
                }
                catch
                {
                    // ignore refused connection
                }

                Console.WriteLine($"Waiting for {name}... retry {i + 1}/{maxRetries}");
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }

            Console.WriteLine($"⚠️ {name} did not respond after {maxRetries} retries.");
        }

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ApiGateway",
                    Version = "v1",
                    Description = """
                        Entry point for all TeaShop micro-services.

                        • Select a cluster (e.g. **OrderService V1**) in the drop-down
                          to view its endpoints routed through the gateway.

                        Example workflow with events:
                        Register -> UserRegisteredEvent -> Add to cart ->
                        Checkout -> CartCheckedOutEvent -> OrderService -> OrderCreatedEvent ->
                        Pay -> OrderPaidEvent -> email receipt

                        ⚠️  Downstream service docs may take a few seconds after
                           startup to appear.
                        """
                });
            });



            var servicesSection = builder.Configuration.GetSection("ReverseProxy:Services");

            foreach (var service in servicesSection.GetChildren())
            {
                //Debug.Print(service.Key + " key from the foreach loop");
                var name = service.Key;
                var address = service.Value;
                if (!string.IsNullOrWhiteSpace(address))
                {
                    builder.Services.AddHttpClient(service.Key, client =>
                    {
                        client.BaseAddress = new Uri(address);
                    });
                    // waiting for services to start listening
                    var healthUrl = $"{address.TrimEnd('/')}";
                    await WaitForService(name, healthUrl);
                }
            }

            var rpSection = builder.Configuration.GetSection("ReverseProxy");

            builder.Services
                .AddReverseProxy()
                .LoadFromConfig(rpSection)
                .AddSwagger(rpSection);

            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            var app = builder.Build();

            app.UseSwagger();

            app.UseSwaggerUI(ui =>
            {
                ui.DocumentTitle = "TeaShop – API Gateway";

                ui.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiGateway V1");
                ui.SwaggerEndpoint("/swagger/OrderServiceCluster/swagger.json", "OrderService V1");
                ui.SwaggerEndpoint("/swagger/UserServiceCluster/swagger.json", "UserService V1");
                ui.SwaggerEndpoint("/swagger/CatalogServiceCluster/swagger.json", "CatalogService V1");
                ui.SwaggerEndpoint("/swagger/CartServiceCluster/swagger.json", "CartService V1");
                ui.SwaggerEndpoint("/swagger/PaymentServiceCluster/swagger.json", "PaymentService V1");


                ui.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapReverseProxy();

            await app.RunAsync();
        }
    }
}
