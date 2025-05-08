using System.Linq.Expressions;
using AuthNexus.Domain.Repositories;
using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthNexus.Infrastructure.Repositories;

/// <summary>
/// 基于Entity Framework Core的通用仓储基类
/// </summary>
public abstract class EFCoreGenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AuthNexusDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    protected EFCoreGenericRepository(AuthNexusDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = dbContext.Set<T>();
    }

    /// <summary>
    /// 获取所有实体
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// 根据条件获取实体
    /// </summary>
    public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// 分页获取实体
    /// </summary>
    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        var query = _dbSet.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync();

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    public virtual async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    public virtual async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    public virtual async Task DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    /// <summary>
    /// 检查是否存在满足条件的实体
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}