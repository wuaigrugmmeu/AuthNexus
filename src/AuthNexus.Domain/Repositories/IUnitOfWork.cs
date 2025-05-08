namespace AuthNexus.Domain.Repositories
{
    /// <summary>
    /// 工作单元接口 - 用于管理事务
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync();
        
        /// <summary>
        /// 开始事务
        /// </summary>
        Task BeginTransactionAsync();
        
        /// <summary>
        /// 提交事务
        /// </summary>
        Task CommitTransactionAsync();
        
        /// <summary>
        /// 回滚事务
        /// </summary>
        Task RollbackTransactionAsync();
    }
}