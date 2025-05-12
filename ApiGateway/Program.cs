using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger.Extensions;
using System.Diagnostics;


namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
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
                        This is the entry point for all TeaShop micro-services.

                        • Select **OrderService V1** in the drop-down (top right) to browse
                          the OrderService endpoints routed through the `/orders` prefix.

                        • The default “Gateway” doc is intentionally empty – it only serves
                          as a starting page.

                        ⚠️  It can take a few seconds after startup for downstream service
                           Swagger documents to become available, so changing the swagger might result in an error.
                        """
                });
            });



            var servicesSection = builder.Configuration.GetSection("ReverseProxy:Services");

            foreach (var service in servicesSection.GetChildren())
            {
                Debug.Print(service.Key + " key from the foreach loop");
                var address = service.Value;
                if (!string.IsNullOrWhiteSpace(address))
                {
                    builder.Services.AddHttpClient(service.Key, client =>
                    {
                        client.BaseAddress = new Uri(address);
                    });
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

                ui.DefaultModelsExpandDepth(-1);
            });

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapReverseProxy();

            app.Run();
        }
    }
}
