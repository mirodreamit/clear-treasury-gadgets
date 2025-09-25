using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class GadgetCategory : BaseEntity
{
    public Guid GadgetId { get; set; }
    public Guid CategoryId { get; set; }
    public int Ordinal { get; set; }
    public Guid LastModifiedByUserId { get; set; }

    public virtual Gadget Gadget { get; set; }
    public virtual Category Category { get; set; }
    public virtual User LastModifiedByUser { get; set; }

    public GadgetCategory()
    { 
    }
    public GadgetCategory(Guid id, Guid gadgetId, Guid categoryId, int ordinal, Guid lastModifiedByUserId): this()
    {
        Id = id;
        GadgetId = gadgetId;
        CategoryId = categoryId;
        Ordinal = ordinal;
        LastModifiedByUserId = lastModifiedByUserId;
    }
}
