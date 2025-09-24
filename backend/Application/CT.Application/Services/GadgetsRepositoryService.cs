using CT.Application.Interfaces;
using CT.Domain.Entities;
using CT.Repository;
using CT.Repository.Abstractions.Interfaces;
using CT.Repository.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace CT.Application.Services;

public class GadgetsRepositoryService(GadgetsDbContext dbContext, ILogger<IRepositoryService<GadgetsDbContext>> logger) : RepositoryService<GadgetsDbContext>(dbContext, logger), IGadgetsRepositoryService
{
    public async Task<int> DecreaseGadgetStockQuantityAsync(Guid gadgetId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, -1, cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> IncreaseGadgetStockQuantityAsync(Guid gadgetId, CancellationToken cancellationToken)
    {
        return await ChangeGadgetStockQuantityAsync(gadgetId, 1, cancellationToken).ConfigureAwait(false);
    }

    // Semaphore to lock access - concurrent stock changes
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    #region private methods
    private async Task<int> ChangeGadgetStockQuantityAsync(Guid gadgetId, int delta, CancellationToken cancellationToken)
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

            await ctx.Gadget
                .Where(g => g.Id == gadgetId)
                .ExecuteUpdateAsync(g => g.SetProperty(x => x.StockQuantity, x => stockQuantity), cancellationToken: cancellationToken).ConfigureAwait(false);

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
