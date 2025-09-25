using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Repository.IS.TypeConfigurations;

public class UserDetailTypeConfiguration : EntityTypeConfigurationBase<UserDetail>
{
    public override string TableName => nameof(UserDetail);

    public override void ConfigureEntity(EntityTypeBuilder<UserDetail> builder)
    {
        base.ConfigureEntity(builder);

        builder
            .HasIndex(p => new { p.Email })
            .IncludeProperties(x => new { x.DisplayName, x.CreatedAt, x.UpdatedAt })
            .IsUnique();

        builder
            .HasOne(d => d.User)
            .WithOne()
            .HasForeignKey<UserDetail>(d => d.Id);
    }
}
