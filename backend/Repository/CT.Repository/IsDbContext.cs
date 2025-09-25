using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Extensions;
using CT.Repository.IS.TypeConfigurations;

namespace CT.Repository;

public class IsDbContext(DbContextOptions<IsDbContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<UserDetail> UserDetail { get; set; }
    public DbSet<UserCredential> UserCredential { get; set; }
    public DbSet<AnonymousUser> AnonymousUser { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("IdentityServer");
        modelBuilder.RemoveOneToManyCascade();

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserDetailTypeConfiguration))!);

        base.OnModelCreating(modelBuilder);
    }
}
