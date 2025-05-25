using Microsoft.Extensions.DependencyInjection;


namespace UserService.Application.Extensions
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));

            return services;
        }
    }
}
