using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Models;

namespace CT.Repository.TypeConfigurations;

public class GadgetCategoryTypeConfiguration : EntityTypeConfigurationBase<GadgetCategory>
{
    public override string TableName => nameof(GadgetCategory);

    public override void ConfigureEntity(EntityTypeBuilder<GadgetCategory> builder)
    {
        base.ConfigureEntity(builder);
        
        // FKs
        builder.HasOne(qq => qq.Gadget)  
             .WithMany(q => q.GadgetCategories)
             .HasForeignKey(o => o.GadgetId);

        builder.HasOne(q => q.Category)
             .WithMany(qq => qq.GadgetCategories)
             .HasForeignKey(o => o.CategoryId);

        // search
        builder
            .HasIndex(p => new { p.GadgetId, p.Ordinal})
            .IncludeProperties(p => new { p.CreatedAt, p.UpdatedAt, p.CategoryId }); 

        // duplicates
        builder
            .HasIndex(p => new { p.GadgetId, p.CategoryId })
            .IsUnique(true); 
    }
}
