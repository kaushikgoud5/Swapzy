using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Swapzy.Infrastructure.Data;

public class SwapzyDbContextFactory
    : IDesignTimeDbContextFactory<SwapzyDbContext>
{
    public SwapzyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SwapzyDbContext>();

        optionsBuilder.UseSqlServer(
            "Server = (localdb)\\MSSQLLocalDB; Database = SwapzyDb_V1 ; Trusted_Connection = True ; MultipleActiveResultSets=True; TrustServerCertificate=True;"
        );

        return new SwapzyDbContext(optionsBuilder.Options);
    }
}
