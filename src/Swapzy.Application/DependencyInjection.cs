using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // Add your core services here
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            //services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<
                Microsoft.AspNetCore.Identity.IPasswordHasher<UserEntity>,
                PasswordHasher<UserEntity>>();
            return services;
        }
    }
}
