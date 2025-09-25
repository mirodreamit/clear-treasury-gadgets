using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Extensions;

namespace CT.Repository;

public class GadgetsDbContext(DbContextOptions<GadgetsDbContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<Gadget> Gadget { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<GadgetCategory> GadgetCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("GD");
        modelBuilder.RemoveOneToManyCascade();

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
