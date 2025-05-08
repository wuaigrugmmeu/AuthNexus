using System.Linq.Expressions;

namespace AuthNexus.Domain.Repositories
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync();
        
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 分页获取实体
        /// </summary>
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// 添加实体
        /// </summary>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// 更新实体
        /// </summary>
        Task UpdateAsync(T entity);
        
        /// <summary>
        /// 删除实体
        /// </summary>
        Task DeleteAsync(T entity);
        
        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        Task DeleteByIdAsync(Guid id);
        
        /// <summary>
        /// 检查是否存在满足条件的实体
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}