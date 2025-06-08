using CartService.Application.Services;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CartService.Infrastructure.Cache;
using CartService.Infrastructure.Messaging;

namespace CartService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<ICartService, Application.Services.CartService>();

        services.AddScoped<ICartRepository, RedisCartRepository>();

        services.AddSingleton<IProductCache, ProductCache>();
        services.AddHostedService<ProductCacheConsumer>();

        return services;
    }
}
