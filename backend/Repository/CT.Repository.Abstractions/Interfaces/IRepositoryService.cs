using CT.Domain.Abstractions.Interfaces;
using CT.Repository.Abstractions.Enums;
using CT.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace CT.Repository.Abstractions.Interfaces;

public interface IRepository<TDbContext> where TDbContext : DbContext
{
    TDbContext DbContext { get; }

    Task<T?> GetByIdAsync<T>(Guid id) where T : class, IBaseEntity;
    Task<T?> GetSingleAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;
    Task<List<T>> GetListAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;
    Task<List<T>> GetListAsync<T, TKey>(Expression<Func<T, TKey>>? orderByAsc = null, Expression<Func<T, TKey>>? orderByDesc = null, int maxRecords = -1, params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;
    Task<List<T>> GetAllAsync<T>() where T : class, IBaseEntity;
    Task<Guid?> GetIdAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;
    Task<ExecuteQueryResponse<TResponse>> QueryAsync<TResponse>(IQueryable<TResponse> query, int pageIndex = 0, int pageSize = -1) where TResponse : class;

    Task<Guid> AddAsync<T>(T entity) where T : class, IBaseEntity;
    Task AddRangeAsync<T>(List<T> entities) where T : class, IBaseEntity;
    Task UpdateAsync<T>(T entity) where T : class, IBaseEntity;
    Task<UpsertEntityResult> UpsertAsync<T>(T entity) where T : class, IBaseEntity;

    Task DeleteAsync<T>(Guid id) where T : class, IBaseEntity;
    Task DeleteRangeAsync<T>(IEnumerable<Guid> ids) where T : class, IBaseEntity;
    Task DeleteWhereAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;

    Task DeleteHardAsync<T>(Guid id) where T : class, IBaseEntity;
    Task DeleteHardRangeAsync<T>(IEnumerable<Guid> ids) where T : class, IBaseEntity;
    Task DeleteHardWhereAsync<T>(params Expression<Func<T, bool>>[] predicates) where T : class, IBaseEntity;

    Task<TransactionModel> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task CommitTransactionAsync(TransactionModel transaction);
    Task RollbackTransactionAsync(TransactionModel transaction);
}
