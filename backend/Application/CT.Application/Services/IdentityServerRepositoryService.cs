using CT.Application.Interfaces;
using CT.Repository;
using CT.Repository.Services;
using Microsoft.Extensions.Logging;

namespace CT.Application.Services;

public class IdentityServerRepositoryService(IsDbContext dbContext, ILogger<RepositoryService<IsDbContext>> logger) : RepositoryService<IsDbContext>(dbContext, logger), IIdentityServerRepositoryService
{
}
