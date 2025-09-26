using CT.Repository;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Application.Interfaces;

public interface IGadgetsRepositoryService: IRepository<GadgetsDbContext>
{
    Task<int> IncreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken);
    Task<int> DecreaseGadgetStockQuantityAsync(Guid gadgetId, Guid userId, CancellationToken cancellationToken);
}
