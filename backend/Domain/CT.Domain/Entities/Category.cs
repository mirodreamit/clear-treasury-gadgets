using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class Category : BaseEntity
{
    public Category()
    { 
    }
    public Category(Guid id, string name, Guid lastModifiedByUserId) : this()
    {
        Id = id;
        Name = name;
        LastModifiedByUserId = lastModifiedByUserId;    
    }

    public string Name { get; set; }
    public Guid LastModifiedByUserId { get; set; }

    public virtual User LastModifiedByUser { get; set; }
    public virtual ICollection<GadgetCategory> GadgetCategories { get; set; }
}
