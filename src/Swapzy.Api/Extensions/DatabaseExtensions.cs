using Microsoft.EntityFrameworkCore;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddSwapzyDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<SwapzyDbContext>(options =>
            options.UseNpgsql(connectionString,
                x => x.UseNetTopologySuite()
                      .EnableRetryOnFailure(3)));

        return services;
    }
}
