using CartService.Application.Interfaces;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Persistence;
using CartService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddScoped<ICartDbContext>(provider => provider.GetRequiredService<CartDbContext>());
        services.AddScoped<ICartRepository, CartRepository>();
        return services;
    }
}
