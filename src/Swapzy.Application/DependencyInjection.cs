using Microsoft.Extensions.DependencyInjection;
using Swapzy.Application.Interfaces;
using Swapzy.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // Add your core services here
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
