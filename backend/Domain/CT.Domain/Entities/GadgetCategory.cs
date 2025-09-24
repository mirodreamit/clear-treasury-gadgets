using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public class GadgetCategory : BaseEntity
{
    public Guid GadgetId { get; set; }
    public Guid CategoryId { get; set; }
    public int Ordinal { get; set; }

    public virtual Gadget Gadget { get; set; }
    public virtual Category Category { get; set; }

    public GadgetCategory(Guid id, Guid gadgetId, Guid categoryId, int ordinal)
    {
        Id = id;
        GadgetId = gadgetId;
        CategoryId = categoryId;
        Ordinal = ordinal;
    }
}
