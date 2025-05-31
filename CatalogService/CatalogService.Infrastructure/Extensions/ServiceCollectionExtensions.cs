using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Persistance;
using CatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());

            services.AddScoped<ICatalogRepository, CatalogRepository>();

            return services;
        }
    }
}