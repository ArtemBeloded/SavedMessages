using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SavedMessages.Application.Data;
using SavedMessages.Application.Users;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Users;
using SavedMessages.Persistence.Repositories;
using SavedMessages.Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<ITokenProvider, TokenProvider>();

            services.AddScoped<IMessageFileRepository, MessageFileRepository>();

            return services;
        }
    }
}
