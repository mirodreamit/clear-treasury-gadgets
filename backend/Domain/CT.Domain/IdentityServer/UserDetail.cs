using CT.Domain.Abstractions.Entities;

namespace CT.Domain.IdentityServer;

public class UserDetail : BaseEntity
{
    public string Email { get; set; }
    public string DisplayName { get; set; }

    public virtual User? User { get; set; }
}