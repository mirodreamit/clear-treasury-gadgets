using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Repository.IS.TypeConfigurations;

public class UserTypeConfiguration : EntityTypeConfigurationBase<User>
{
    public override string TableName => nameof(User);

    public override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        base.ConfigureEntity(builder);

        builder
          .HasIndex(p => new { p.Identifier })
          .IncludeProperties(x => new { x.IsSuperAdmin, x.CreatedAt, x.UpdatedAt, x.IsBlocked })
          .IsUnique();
    }
}
