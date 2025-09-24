using Microsoft.EntityFrameworkCore;
using CT.Domain.Abstractions.Interfaces;
using CT.Repository.Abstractions.Enums;
using CT.Repository.Abstractions.Models;
using System.Data;
using System.Linq.Expressions;

namespace CT.Repository.Abstractions.Interfaces;

public interface IRepositoryService<TDbContext> where TDbContext : DbContext
{
    TDbContext DbContext { get; }
    Task<T?> GetEntityByIdAsync<T>(Guid id) where T : class, IBaseEntity;

    Task<Guid?> GetEntityIdByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task<T?> GetEntityByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task<List<T>> GetEntitiesByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task<List<T>> GetEntitiesByExpressionAsync<T, TKey>(Expression<Func<T, TKey>>? orderByAscending = null, Expression<Func<T, TKey>>? orderByDescending = null, int maxRecordCount = -1, params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task<List<T>> GetAllEntitiesAsync<T>() where T : class, IBaseEntity;

    Task<ExecuteQueryResponse<TResponse>> ExecuteQueryAsync<TResponse>(IQueryable<TResponse> query, int pageIndex = 0, int pageSize = -1) where TResponse : class;

    Task<Guid> InsertEntityAsync<T>(T entity) where T : class, IBaseEntity;

    Task UpdateEntityAsync<T>(T entity) where T : class, IBaseEntity;

    Task<UpsertEntityResult> UpsertEntityAsync<T>(T entity) where T : class, IBaseEntity;

    Task DeleteEntityAsync<T>(Guid id) where T : class, IAuditableEntityWithSoftDelete, IBaseEntity;

    Task DeleteEntitiesHardAsync<T>(List<Guid> ids) where T : class, IBaseEntity;
    Task DeleteEntityHardAsync<T>(Guid id) where T : class, IBaseEntity;

    Task DeleteEntityHardByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task DeleteEntitiesHardByExpressionAsync<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IBaseEntity;

    Task InsertEntitiesAsync<T>(List<T> entities) where T : class, IBaseEntity;

    Task<TransactionModel> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    Task CommitTransactionAsync(TransactionModel transactionModel);

    Task RollbackTransactionAsync(TransactionModel transactionModel);
}
