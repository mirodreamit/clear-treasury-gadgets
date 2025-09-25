using CT.Repository;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Application.Interfaces;

public interface IIdentityServerRepositoryService : IRepository<IsDbContext>
{
}
