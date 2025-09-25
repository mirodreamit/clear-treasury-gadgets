using CT.Domain.Abstractions.Entities;

namespace CT.Domain.IdentityServer;
public class UserCredential : BaseEntity
{
    public string PasswordHash { get; set; }

    public virtual User? Principal { get; set; }
}
