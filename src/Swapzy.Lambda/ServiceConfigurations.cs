using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swapzy.Application;
using Swapzy.Application.Interfaces;
using Swapzy.Application.Services;
using Swapzy.Core.Entities.Users;
using Swapzy.Infrastructure.Data;
using Swapzy.Infrastructure.Repositories;
using Swapzy.Infrastructure.Services;
using System.Text;

namespace Swapzy.Lambda
{
    static class ServiceConfigurations
    {
        public static void Register(ServiceCollection services, IConfigurationRoot configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddDbContext<SwapzyDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddAutoMapper(typeof(ServiceConfigurations).Assembly);
            services.AddScoped<IPasswordHasher<UserEntity>,PasswordHasher<UserEntity>>();
            services.AddDistributedMemoryCache();
            services.AddLogging();
            services.AddScoped<IAuthMiddleware, JwtAuthMiddleware>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();
        }
    }
}
