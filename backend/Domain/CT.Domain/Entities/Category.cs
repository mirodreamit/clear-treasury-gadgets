using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class Category : BaseEntity
{
    public Category(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; set; } 

    public virtual ICollection<GadgetCategory> GadgetCategories { get; set; }
}
