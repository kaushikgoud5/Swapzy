using Microsoft.AspNetCore.Identity;
using Swapzy.Application.Interfaces;
using Swapzy.Application.Services;
using Swapzy.Core.Entities.Users;
using Swapzy.Infrastructure.Repositories;
using Swapzy.Infrastructure.Services;

namespace Swapzy.Api.Extensions;

public static class AppServicesExtensions
{
    public static IServiceCollection AddSwapzyServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISocialAuthService, SocialAuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProximityService, ProximityService>();
        services.AddScoped<IInterestService, InterestService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddHttpClient();
        services.AddDistributedMemoryCache();
        services.AddAutoMapper(typeof(Swapzy.Application.Mappings.MappingProfile).Assembly);

        return services;
    }
}
