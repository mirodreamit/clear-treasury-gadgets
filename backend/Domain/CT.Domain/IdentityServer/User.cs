using CT.Domain.Abstractions.Entities;

namespace CT.Domain.IdentityServer;

public partial class User : BaseEntity
{
    public string Identifier { get; set; }
    public bool IsSuperAdmin { get; set; }
    public bool? IsBlocked { get; set; }
}
