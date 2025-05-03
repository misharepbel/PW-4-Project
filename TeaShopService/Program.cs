using Yarp.ReverseProxy;

namespace TeaShopService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var servicesSection = builder.Configuration.GetSection("Services");

            foreach (var service in servicesSection.GetChildren())
            {
                var address = service.Value;
                if (!string.IsNullOrWhiteSpace(address))
                {
                    builder.Services.AddHttpClient(service.Key, client =>
                    {
                        client.BaseAddress = new Uri(address);
                    });
                }
            }


            //YARP
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapReverseProxy();

            app.Run();
        }
    }
}
