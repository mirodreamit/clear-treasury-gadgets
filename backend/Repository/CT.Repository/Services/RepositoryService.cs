using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using CT.Domain.Abstractions.Interfaces;
using CT.Repository.Abstractions.Enums;
using CT.Repository.Abstractions.Models;
using CT.Repository.Abstractions.Extensions;
using CT.Repository.Abstractions.Interfaces;

namespace CT.Repository.Services;

public class RepositoryService<TDbContext>(TDbContext dbContext, ILogger<RepositoryService<TDbContext>> logger) : IRepository<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext = dbContext;
    private readonly ILogger<RepositoryService<TDbContext>> _logger = logger;
    
    public TDbContext DbContext => _dbContext;

    #region Query

    public async Task<T?> GetByIdAsync<T>(Guid id) where T : class, IBaseEntity
    {
        return await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<T?> GetSingleAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        var queryable = ApplyExpressions(_dbContext.Set<T>(), predicates);
        return await queryable.AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task<List<T>> GetListAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        var queryable = ApplyExpressions(_dbContext.Set<T>(), predicates);
        return await ExecuteSimpleQueryAsync(queryable);
    }

    public async Task<List<T>> GetListAsync<T, TKey>(Expression<Func<T, TKey>>? orderByAsc = null, Expression<Func<T, TKey>>? orderByDesc = null, int maxRecords = -1, params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        var queryable = ApplyExpressions(_dbContext.Set<T>(), predicates);

        if (orderByAsc != null)
            queryable = queryable.OrderBy(orderByAsc);

        if (orderByDesc != null)
            queryable = queryable.OrderByDescending(orderByDesc);

        return await ExecuteSimpleQueryAsync(queryable, maxRecords);
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class, IBaseEntity
    {
        return await ExecuteSimpleQueryAsync(_dbContext.Set<T>());
    }

    public async Task<Guid?> GetIdAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        var queryable = ApplyExpressions(_dbContext.Set<T>(), predicates);
        return await queryable.AsNoTracking().Select(x => (Guid?)x.Id).SingleOrDefaultAsync();
    }

    public async Task<ExecuteQueryResponse<TResponse>> QueryAsync<TResponse>(IQueryable<TResponse> query, int pageIndex = 0, int pageSize = -1) where TResponse : class
    {
        var skipRecords = CalculateDataPagingRecordsToSkip(pageSize, pageIndex);
        var records = await ExecuteSimpleQueryAsync(query, pageSize, skipRecords).ConfigureAwait(false);

        var totalCount = pageSize != -1
            ? await query.CountAsync().ConfigureAwait(false)
            : records.Count;

        return new ExecuteQueryResponse<TResponse>
        {
            TotalRecordCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            Records = records
        };
    }

    #endregion

    #region Commands

    public async Task<Guid> AddAsync<T>(T entity) where T : class, IBaseEntity
    {
        var entityId = InsertEntityToDbSet(entity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return entityId;
    }

    public async Task AddRangeAsync<T>(List<T> entities) where T : class, IBaseEntity
    {
        foreach (var entity in entities)
            InsertEntityToDbSet(entity);

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<UpsertEntityResult> UpsertAsync<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = await GetByIdAsync<T>(entity.Id);

        if (existing == null)
        {
            await AddAsync(entity);
            return UpsertEntityResult.Inserted;
        }

        _dbContext.DetachLocal(entity, entity.Id);
        await MergeWithExistingAndUpdateEntityAsync(entity, existing);

        return UpsertEntityResult.Updated;
    }

    public async Task DeleteAsync<T>(Guid id) where T : class, IBaseEntity
    {
        var entity = await GetByIdAsync<T>(id) ?? throw new KeyNotFoundException($"Entity not found. [id: {id}]");

        if (entity is IAuditableEntityWithSoftDelete softEntity)
        {
            softEntity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
        }
        else
        {
            throw new InvalidOperationException($"Entity type {typeof(T).Name} does not support soft delete.");
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteRangeAsync<T>(IEnumerable<Guid> ids) where T : class, IBaseEntity
    {
        if (ids == null || !ids.Any())
            return;

        var entities = await _dbContext.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync();

        foreach (var entity in entities.OfType<IAuditableEntityWithSoftDelete>())
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update((T)entity);
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteWhereAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        var entities = await ApplyExpressions(_dbContext.Set<T>(), predicates).ToListAsync();

        foreach (var entity in entities.OfType<IAuditableEntityWithSoftDelete>())
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update((T)entity);
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteHardAsync<T>(Guid id) where T : class, IBaseEntity
    {
        int affected = await _dbContext.Set<T>()
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();

        if (affected == 0)
            throw new KeyNotFoundException($"Entity not found. [id: {id}]");
    }

    public async Task DeleteHardRangeAsync<T>(IEnumerable<Guid> ids) where T : class, IBaseEntity
    {
        if (ids == null || !ids.Any())
            return;

        await _dbContext.Set<T>().Where(e => ids.Contains(e.Id)).ExecuteDeleteAsync().ConfigureAwait(false);
    }

    public async Task DeleteHardWhereAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity
    {
        await ApplyExpressions(_dbContext.Set<T>(), predicates).ExecuteDeleteAsync().ConfigureAwait(false);
    }

    #endregion

    #region Transactions

    public async Task<TransactionModel> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
        return new TransactionModel { Transaction = transaction };
    }

    public async Task CommitTransactionAsync(TransactionModel transactionModel)
    {
        var tran = (IDbContextTransaction)transactionModel.Transaction!;
        await tran.CommitAsync();
    }

    public async Task RollbackTransactionAsync(TransactionModel transactionModel)
    {
        var tran = (IDbContextTransaction)transactionModel.Transaction!;
        await tran.RollbackAsync();
    }

    #endregion

    #region Private helpers

    private static IQueryable<T> ApplyExpressions<T>(IQueryable<T> query, params Expression<Func<T, bool>>[] predicates) where T : class
    {
        foreach (var exp in predicates)
            query = query.Where(exp);

        return query;
    }

    private static async Task<List<TResponse>> ExecuteSimpleQueryAsync<TResponse>(IQueryable<TResponse> query, int maxRecordCount = -1, int skipRecords = 0) where TResponse : class
    {
        query = query.AsNoTracking();

        if (skipRecords > 0)
            query = query.Skip(skipRecords);

        if (maxRecordCount != -1)
            query = query.Take(maxRecordCount);

        return await query.ToListAsync().ConfigureAwait(false);
    }

    private static int CalculateDataPagingRecordsToSkip(int pageSize, int pageIndex) => pageSize > 0 ? pageIndex * pageSize : 0;

    private Guid InsertEntityToDbSet<T>(T entity) where T : class, IBaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _dbContext.Set<T>().Add(entity);
        return entity.Id;
    }

    private async Task MergeWithExistingAndUpdateEntityAsync<T>(T entity, T? existing) where T : class, IBaseEntity
    {
        if (existing != null)
        {
            entity.CreatedAt = existing.CreatedAt;
            entity.UpdatedAt = existing.UpdatedAt;

            if (entity is IAuditableEntityWithSoftDelete softDeleteEntity && existing is IAuditableEntityWithSoftDelete existingSoftDelete)
            {
                softDeleteEntity.IsDeleted = existingSoftDelete.IsDeleted;
            }
        }

        await UpdateAsync(entity);
    }

    #endregion
}
