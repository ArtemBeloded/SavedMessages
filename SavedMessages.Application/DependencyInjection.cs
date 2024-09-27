using Microsoft.Extensions.DependencyInjection;
using SavedMessages.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services) 
        {
            services.AddMediatR(config =>
            {
                //Registers all handlers and other MediatR services that are in the same assembly
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();

                //Register pipline behavior
                config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
            });

            return services;
        }
    }
}
