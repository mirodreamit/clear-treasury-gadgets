using Microsoft.EntityFrameworkCore;
using CT.Domain.Abstractions.Interfaces;

namespace CT.Repository.Abstractions.Extensions;

public static class DbContextExtensions
{
    public static void DetachLocal<T>(this DbContext context, T t, Guid entryId) where T : class, IBaseEntity
    {
        T? val = context.Set<T>().Local.FirstOrDefault((entry) => entry.Id.Equals(entryId));

        if (val != null)
        {
            context.Entry(val).State = EntityState.Detached;
        }

        context.Entry(t).State = EntityState.Detached;
    }
}
