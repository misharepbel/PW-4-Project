
namespace TeaShopService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("http://localhost:5000/swagger/v1/swagger.json", "TeaShopService");
                    c.SwaggerEndpoint("http://localhost:5100/swagger/v1/swagger.json", "OrderService");
                    // c.SwaggerEndpoint("http://localhost:5200/swagger/v1/swagger.json", "InvoiceService");
                    // c.SwaggerEndpoint("http://localhost:5300/swagger/v1/swagger.json", "UserService");
                });

            //}

            app.UseHttpsRedirection();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
