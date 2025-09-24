using CT.Domain.Abstractions.Interfaces;

namespace CT.Domain.Abstractions.Entities;

public class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
