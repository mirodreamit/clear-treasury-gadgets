using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Repository.IS.TypeConfigurations;

public class AnonymousUserTypeConfiguration : EntityTypeConfigurationBase<AnonymousUser>
{
    public override string TableName => nameof(AnonymousUser);

    public override void ConfigureEntity(EntityTypeBuilder<AnonymousUser> builder)
    {
        base.ConfigureEntity(builder);

        builder
            .HasIndex(p => new { p.SessionId })
            .IncludeProperties(x=> new { x.CreatedAt, x.UpdatedAt  })
            .IsUnique();

        builder
            .HasOne(d => d.User)
            .WithOne()
            .HasForeignKey<AnonymousUser>(d => d.Id);
    }
}