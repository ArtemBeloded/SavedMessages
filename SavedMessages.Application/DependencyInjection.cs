using Microsoft.Extensions.DependencyInjection;
using SavedMessages.Application.Behaviors;

namespace SavedMessages.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) 
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();

                config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
            });

            return services;
        }
    }
}
