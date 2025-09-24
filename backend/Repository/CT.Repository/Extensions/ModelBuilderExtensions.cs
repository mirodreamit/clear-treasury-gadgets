using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CT.Repository.Extensions;

public static class ModelBuilderExtensions
{
    public static void RemoveOneToManyCascade(this ModelBuilder builder)
    {
        builder.EntityLoop(delegate (IMutableEntityType et)
        {
            (from fk in et.GetForeignKeys()
             where fk.DeleteBehavior == DeleteBehavior.Cascade
             select fk).ToList().ForEach(delegate (IMutableForeignKey fk)
             {
                 fk.DeleteBehavior = DeleteBehavior.Restrict;
             });
        });
    }

    private static void EntityLoop(this ModelBuilder builder, Action<IMutableEntityType> action)
    {
        foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
        {
            action(entityType);
        }
    }
}
