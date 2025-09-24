using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using CT.Domain.Abstractions.Entities;
using CT.Domain.Abstractions.Interfaces;
using CT.Repository.Abstractions.Enums;
using CT.Repository.Abstractions.Models;
using CT.Repository.Abstractions.Extensions;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Repository.Abstractions.Services;

public class RepositoryService<TDbContext>(DbContext dbContext, ILogger<IRepositoryService<TDbContext>> logger) : IRepositoryService<TDbContext> where TDbContext : DbContext
{
    private DbContext _dbContext = dbContext;

    private readonly ILogger<IRepositoryService<TDbContext>> _logger = logger;

    public TDbContext DbContext
    {
        get
        {
            return (TDbContext)_dbContext;
        }
        set
        {
            _dbContext = value;
        }
    }

    public ILogger<IRepositoryService<TDbContext>> Logger => _logger;

    public async Task<List<T>> GetAllEntitiesAsync<T>() where T : class, IBaseEntity
    {
        DbSet<T> query = _dbContext.Set<T>();
        return await ExecuteSimpleQueryAsync(query);
    }

    public async Task<List<T>> GetEntitiesByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        IQueryable<T> queryable = _dbContext.Set<T>().Where(expressions.First());
        if (expressions.Length > 1)
        {
            foreach (Expression<Func<T, bool>> item in expressions.Skip(1))
            {
                queryable = queryable.Where(item);
            }
        }

        return await ExecuteSimpleQueryAsync(queryable);
    }

    public async Task<List<T>> GetEntitiesByExpressionAsync<T, TKey>(Expression<Func<T, TKey>>? orderByAscending = null, Expression<Func<T, TKey>>? orderByDescending = null, int maxRecordCount = -1, params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        IQueryable<T> queryable = _dbContext.Set<T>().Where(expressions.First());
        if (expressions.Length > 1)
        {
            foreach (Expression<Func<T, bool>> item in expressions.Skip(1))
            {
                queryable = queryable.Where(item);
            }
        }

        if (orderByAscending != null)
        {
            queryable = queryable.OrderBy(orderByAscending);
        }

        if (orderByDescending != null)
        {
            queryable = queryable.OrderByDescending(orderByDescending);
        }

        return await ExecuteSimpleQueryAsync(queryable, maxRecordCount);
    }

    public async Task<T?> GetEntityByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        IQueryable<T> queryable = _dbContext.Set<T>().Where(expressions.First());
        if (expressions.Length > 1)
        {
            foreach (Expression<Func<T, bool>> item in expressions.Skip(1))
            {
                queryable = queryable.Where(item);
            }
        }

        return (await ExecuteSimpleQueryAsync(queryable)).SingleOrDefault();
    }

    public async Task<Guid?> GetEntityIdByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        IQueryable<T> source = _dbContext.Set<T>().Where(expressions.First());
        if (expressions.Length > 1)
        {
            foreach (Expression<Func<T, bool>> item in expressions.Skip(1))
            {
                source = source.Where(item);
            }
        }

        List<Guid> list = await source.Select((x) => x.Id).ToListAsync().ConfigureAwait(continueOnCapturedContext: false);
        Guid? result = null;
        if (list.Count > 0)
        {
            result = list.SingleOrDefault();
        }

        return result;
    }

    public async Task<T?> GetEntityByIdAsync<T>(Guid id) where T : class, IBaseEntity
    {
        IQueryable<T> query = from x in _dbContext.Set<T>()
                              where (x as BaseEntity)!.Id == id
                              select x;
        return (await ExecuteSimpleQueryAsync(query)).FirstOrDefault();
    }

    private Guid InsertEntityToDbSet<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _dbContext.Set<T>().Add(entity);

        return entity.Id;
    }

    public async Task<Guid> InsertEntityAsync<T>(T entity) where T : class, IBaseEntity
    {
        Guid entityId = InsertEntityToDbSet(entity);
        try
        {
            await _dbContext.SaveChangesAsync();
            return entityId;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task InsertEntitiesAsync<T>(List<T> entities) where T : class, IBaseEntity
    {
        foreach (T entity in entities)
        {
            InsertEntityToDbSet(entity);
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateEntityAsync<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _dbContext.Set<T>().Update(entity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<UpsertEntityResult> UpsertEntityAsync<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        UpsertEntityResult result;

        switch (_dbContext.Entry(entity).State)
        {
            case EntityState.Detached:
            case EntityState.Unchanged:
                {
                    T? val = await GetEntityByIdAsync<T>(entity.Id);

                    if (val == null)
                    {
                        await InsertEntityAsync(entity);

                        result = UpsertEntityResult.Inserted;
                    }
                    else
                    {
                        _dbContext.DetachLocal(entity, entity.Id);

                        await MergeWithExistingAndUpdateEntityAsync(entity, val);

                        result = UpsertEntityResult.Updated;
                    }

                    break;
                }
            case EntityState.Modified:
                await MergeWithExistingAndUpdateEntityAsync(entity, await GetEntityByIdAsync<T>(entity.Id));

                result = UpsertEntityResult.Updated;

                break;
            case EntityState.Added:
                await InsertEntityAsync(entity);

                result = UpsertEntityResult.Inserted;

                break;
            default:
                throw new Exception("Unknown entity state.");
        }

        return result;
    }

    public async Task DeleteEntityAsync<T>(Guid id) where T : class, IAuditableEntityWithSoftDelete, IBaseEntity
    {
        T val = await GetEntityByIdAsync<T>(id) ?? throw new KeyNotFoundException($"Entity with the given key not found. [id: {id}]");
        val.IsDeleted = true;
        await UpdateEntityAsync(val);
    }

    public async Task DeleteEntityHardAsync<T>(Guid id) where T : class, IBaseEntity
    {
        T val = await GetEntityByIdAsync<T>(id) ?? throw new KeyNotFoundException($"Entity with the given key not found. [id: {id}]");
        _dbContext.DetachLocal(val, val.Id);
        _dbContext.Remove(val);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteEntitiesHardAsync<T>(List<Guid> ids) where T : class, IBaseEntity
    {
        if (ids == null || ids.Count == 0)
            return;

        await _dbContext.Set<T>()
            .Where(e => ids.Contains(e.Id))
            .ExecuteDeleteAsync().ConfigureAwait(false);
    }

    public async Task DeleteEntityHardByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        List<T> obj = await GetEntitiesByExpressionAsync(expressions);
        if (obj.Count > 1)
        {
            throw new Exception("Retrieved multiple entities while expecting one.");
        }

        T val = obj.SingleOrDefault() ?? throw new Exception("Entity not found by expression.");
        _dbContext.DetachLocal(val, val.Id);
        _dbContext.Remove(val);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteEntitiesHardByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity
    {
        List<T> obj = await GetEntitiesByExpressionAsync(expressions);

        foreach (T entity in obj)
        {
            _dbContext.DetachLocal(entity, entity.Id);
            _dbContext.Remove(entity);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task MergeWithExistingAndUpdateEntityAsync<T>(T entity, T? existing) where T : class, IBaseEntity
    {
        if (existing != null)
        {
            entity.CreatedAt = existing.CreatedAt;
            entity.UpdatedAt = existing.UpdatedAt;

            if (entity is IAuditableEntityWithSoftDelete auditableEntityWithSoftDelete && existing is IAuditableEntityWithSoftDelete auditableEntityWithSoftDelete2)
            {
                auditableEntityWithSoftDelete.IsDeleted = auditableEntityWithSoftDelete2.IsDeleted;
            }
        }

        await UpdateEntityAsync(entity);
    }

    public async Task<TransactionModel> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);

        return new TransactionModel
        {
            Transaction = transaction
        };
    }

    public async Task CommitTransactionAsync(TransactionModel transactionModel)
    {
        var tran = transactionModel.Transaction as IDbContextTransaction;

        await tran!.CommitAsync();
    }

    public async Task RollbackTransactionAsync(TransactionModel transactionModel)
    {
        var tran = transactionModel.Transaction as IDbContextTransaction;

        await tran!.RollbackAsync();
    }

    public void DetachLocal<TEntity>(TEntity t, Guid entryId) where TEntity : class, IBaseEntity
    {
        _dbContext.DetachLocal(t, entryId);
    }

    protected async Task<int> ExecuteQueryCountAsync<TResponse>(IQueryable<TResponse> query) where TResponse : class
    {
        return await query.CountAsync();
    }

    protected async Task<List<TResponse>> ExecuteSimpleQueryAsync<TResponse>(IQueryable<TResponse> query, int maxRecordCount = -1, int skipRecords = 0) where TResponse : class
    {
        query = query.AsNoTracking();

        if (skipRecords > 0)
        {
            query = query.Skip(skipRecords);
        }

        if (maxRecordCount != -1)
        {
            query = query.Take(maxRecordCount);
        }

        var result = await query.ToListAsync().ConfigureAwait(continueOnCapturedContext: false);

        return result;
    }

    private static int CalculateDataPagingRecordsToSkip(int pageSize, int pageIndex)
    {
        int result = 0;
        if (pageSize > 0)
        {
            result = pageIndex * pageSize;
        }

        return result;
    }

    public async Task<ExecuteQueryResponse<TResponse>> ExecuteQueryAsync<TResponse>(IQueryable<TResponse> query, int pageIndex = 0, int pageSize = -1) where TResponse : class
    {
        int skipRecords = RepositoryService<TDbContext>.CalculateDataPagingRecordsToSkip(pageSize, pageIndex);
        
        List<TResponse> records = await ExecuteSimpleQueryAsync(query, pageSize, skipRecords).ConfigureAwait(continueOnCapturedContext: false);
        
        int num = records.Count;

        if (pageSize != -1)
        {
            num = await ExecuteQueryCountAsync(query).ConfigureAwait(continueOnCapturedContext: false);
        }

        return new ExecuteQueryResponse<TResponse>()
        {
            TotalRecordCount = num,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Records = records
        };
    }


}