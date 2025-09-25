using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class Gadget: BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }

    public Guid LastModifiedByUserId { get; set; }

    public Gadget()
    { 
    }
    public Gadget(Guid id): this()
    {
        Id = id;
    }
    public Gadget(Guid id, string name, int stockQuantity, string? description, Guid lastModifiedByUserId): this(id)
    {
        Name = name;
        StockQuantity = stockQuantity;
        Description = description;
        LastModifiedByUserId = lastModifiedByUserId;
    }

    public virtual User LastModifiedByUser { get; set; }
    public virtual ICollection<GadgetCategory> GadgetCategories { get; set; }
}
