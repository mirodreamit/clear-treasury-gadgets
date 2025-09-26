using CT.Application.Abstractions.Exceptions;
using CT.Application.Interfaces;
using CT.Repository;
using CT.Repository.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CT.Application.Services;

public class GadgetsRepositoryService(GadgetsDbContext dbContext, ILogger<RepositoryService<GadgetsDbContext>> logger) : RepositoryService<GadgetsDbContext>(dbContext, logger), IGadgetsRepositoryService
{
    public async Task<int> DecreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, userId, -1, cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> IncreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, userId, 1, cancellationToken).ConfigureAwait(false);
    }

    // Semaphore to lock access - concurrent stock changes
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    #region private methods
    private async Task<int> ChangeGadgetStockQuantityAsync(Guid gadgetId, Guid userId, int delta, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            var ctx = DbContext;

            var stockQuantity = await ctx.Gadget
                .Where(g => g.Id == gadgetId)
                .Select(g => g.StockQuantity)
                .FirstAsync(cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            stockQuantity += delta;

            if (stockQuantity < 0)
            {
                throw new UserFriendlyException("Stock quantity cannot be less than zero.");
            }

            await ctx.Gadget
                .Where(g => g.Id == gadgetId)
                .ExecuteUpdateAsync(g => g
                        .SetProperty(x => x.StockQuantity, x => stockQuantity)
                        .SetProperty(x => x.LastModifiedByUserId, x => userId), cancellationToken: cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            return stockQuantity;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    #endregion
}
