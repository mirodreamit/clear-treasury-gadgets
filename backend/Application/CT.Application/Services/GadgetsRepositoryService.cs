using CT.Application.Interfaces;
using CT.Repository;
using CT.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CT.Application.Services;

public class GadgetsRepositoryService(GadgetsDbContext dbContext, ILogger<RepositoryService<GadgetsDbContext>> logger) : RepositoryService<GadgetsDbContext>(dbContext, logger), IGadgetsRepositoryService
{
    public async Task<(bool Success, string? ErrorMessage, int StockQuantity)> DecreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, userId, -1, cancellationToken).ConfigureAwait(false);
    }

    public async Task<(bool Success, string? ErrorMessage, int StockQuantity)> IncreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, userId, 1, cancellationToken).ConfigureAwait(false);
    }

    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    #region private methods
    private async Task<(bool Success, string? ErrorMessage, int StockQuantity)> ChangeGadgetStockQuantityAsync(
        Guid gadgetId, Guid userId, int delta, CancellationToken cancellationToken)
    {
        var semaphore = _locks.GetOrAdd(gadgetId, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        try
        {
            var ctx = DbContext;

            var stockQuantity = await ctx.Gadget
                .Where(g => g.Id == gadgetId)
                .Select(g => g.StockQuantity)
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            var newStockQuantity = stockQuantity + delta;

            if (newStockQuantity < 0)
            {
                return (false, "Stock quantity cannot be less than zero.", stockQuantity);
            }

            await ctx.Gadget
                .Where(g => g.Id == gadgetId)
                .ExecuteUpdateAsync(g => g
                    .SetProperty(x => x.StockQuantity, x => newStockQuantity)
                    .SetProperty(x => x.LastModifiedByUserId, x => userId),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return (true, null, newStockQuantity);
        }
        finally
        {
            semaphore.Release();
        }
    }
#endregion
}
