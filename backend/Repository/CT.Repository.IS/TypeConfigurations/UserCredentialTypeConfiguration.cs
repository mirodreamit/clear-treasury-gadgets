using CT.Domain.IdentityServer;
using CT.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CT.Repository.IS.TypeConfigurations;

public class UserCredentialTypeConfiguration : EntityTypeConfigurationBase<UserCredential>
{
    public override string TableName => nameof(UserCredential);

    public override void ConfigureEntity(EntityTypeBuilder<UserCredential> builder)
    {
        base.ConfigureEntity(builder);
        
        builder
            .HasOne(d => d.Principal)
            .WithOne()
            .HasForeignKey<UserCredential>(d => d.Id);
    }
}
