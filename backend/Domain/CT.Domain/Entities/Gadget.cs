using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class Gadget: BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }

    public Gadget(Guid id)
    {
        Id = id;
    }
    public Gadget(Guid id, string name, int stockQuantity, string? description): this(id)
    {
        Name = name;
        StockQuantity = stockQuantity;
        Description = description;
    }

    public virtual ICollection<GadgetCategory> GadgetCategories { get; set; }
}
