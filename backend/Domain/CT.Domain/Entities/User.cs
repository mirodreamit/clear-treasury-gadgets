using CT.Domain.Abstractions.Entities;

namespace CT.Domain.Entities;

public partial class User : BaseEntity
{
    public string Identifier { get; set; }
    
    public virtual ICollection<Gadget> Gadgets { get; set; }
    public virtual ICollection<GadgetCategory> GadgetCategoriess { get; set; }
    public virtual ICollection<Category> Categories { get; set; }
}
