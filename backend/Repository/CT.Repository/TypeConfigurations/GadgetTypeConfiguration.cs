using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CT.Domain.Entities;
using CT.Repository.Abstractions.Models;

namespace CT.Repository.TypeConfigurations;

public class GadgetTypeConfiguration : EntityTypeConfigurationBase<Gadget>
{
    public override string TableName => nameof(Gadget);

    public override void ConfigureEntity(EntityTypeBuilder<Gadget> builder)
    {
        base.ConfigureEntity(builder);


        // FKs
        builder.HasOne(qq => qq.LastModifiedByUser)
             .WithMany(q => q.Gadgets)
             .HasForeignKey(o => o.LastModifiedByUserId); 

        // indexes
        builder
            .HasIndex(p => new { p.UpdatedAt })
            .IncludeProperties(p => new { p.Name, p.CreatedAt });

        builder
            .HasIndex(p => new { p.Name })
            .IncludeProperties(p => new { p.CreatedAt, p.UpdatedAt })
            .IsUnique(true);
    }
}
