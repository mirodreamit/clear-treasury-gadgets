using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CT.Repository;

namespace CT.Repository.Factories;

public class IsDbContextFactory : IDesignTimeDbContextFactory<IsDbContext>
{
    public IsDbContext CreateDbContext(string[] args)
    {
        string? connectionString = args.Length != 0 ? args[0] : null;

        var dbOptions = GenerateDbOptions(connectionString);

        return new IsDbContext(dbOptions);
    }

    private static DbContextOptions<IsDbContext> GenerateDbOptions(string? connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IsDbContext>();

        if (string.IsNullOrEmpty(connectionString))
        {
            optionsBuilder.UseSqlServer(o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
        }

        return optionsBuilder.Options;
    }
}
