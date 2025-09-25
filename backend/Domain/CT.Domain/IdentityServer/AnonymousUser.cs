using CT.Domain.Abstractions.Entities;

namespace CT.Domain.IdentityServer;

public class AnonymousUser : BaseEntity
{
    public string SessionId { get; set; }
    
    public virtual User? User { get; set; }
}
