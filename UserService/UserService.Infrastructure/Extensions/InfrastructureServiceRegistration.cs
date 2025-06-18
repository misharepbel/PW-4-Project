using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Messaging;

namespace UserService.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("Default")));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IUserRegisteredProducer, KafkaUserRegisteredProducer>();
        services.AddSingleton<IPasswordResetProducer, KafkaPasswordResetProducer>();

        return services;
    }
}
