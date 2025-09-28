using CT.Repository;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Application.Interfaces;

public interface IGadgetsRepositoryService: IRepository<GadgetsDbContext>
{
    Task<(bool Success, string? ErrorMessage, int StockQuantity)> IncreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken);
    Task<(bool Success, string? ErrorMessage, int StockQuantity)> DecreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken);
}
