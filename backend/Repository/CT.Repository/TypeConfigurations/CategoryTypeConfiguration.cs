using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Models;

namespace CT.Repository.TypeConfigurations;

public class CategoryTypeConfiguration : EntityTypeConfigurationBase<Category>
{
    public override string TableName => nameof(Category);

    public override void ConfigureEntity(EntityTypeBuilder<Category> builder)
    {
        base.ConfigureEntity(builder);

        builder
            .HasIndex(p => new { p.UpdatedAt })
            .IncludeProperties(p => new { p.Name, p.CreatedAt });

        builder
            .HasIndex(p => new { p.Name })
            .IncludeProperties(p => new { p.CreatedAt, p.UpdatedAt })
            .IsUnique(true);
    }
}
