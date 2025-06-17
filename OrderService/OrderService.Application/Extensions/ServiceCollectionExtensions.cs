using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Services;

namespace OrderService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        services.AddScoped<IOrderService, Services.OrderService>();
        return services;
    }
}
