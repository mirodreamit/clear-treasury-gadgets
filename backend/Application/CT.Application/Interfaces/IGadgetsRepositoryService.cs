using CT.Repository;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Application.Interfaces;

public interface IGadgetsRepositoryService: IRepositoryService<GadgetsDbContext>
{
    Task<int> IncreaseGadgetStockQuantityAsync(Guid gadgetId, CancellationToken cancellationToken);
    Task<int> DecreaseGadgetStockQuantityAsync(Guid gadgetId, CancellationToken cancellationToken);
}
