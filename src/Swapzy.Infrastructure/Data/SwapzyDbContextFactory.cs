using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Swapzy.Infrastructure.Data;

public class SwapzyDbContextFactory : IDesignTimeDbContextFactory<SwapzyDbContext>
{
    public SwapzyDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Swapzy.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<SwapzyDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new SwapzyDbContext(optionsBuilder.Options);
    }
}
