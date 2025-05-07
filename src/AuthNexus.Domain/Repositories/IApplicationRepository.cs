using AuthNexus.Domain.Entities;

namespace AuthNexus.Domain.Repositories;

/// <summary>
/// 应用仓储接口
/// </summary>
public interface IApplicationRepository
{
    /// <summary>
    /// 根据ID获取应用
    /// </summary>
    Task<Application> GetByIdAsync(Guid id);
    
    /// <summary>
    /// 根据AppUID获取应用
    /// </summary>
    Task<Application> GetByAppUidAsync(string appUid);
    
    /// <summary>
    /// 验证应用访问凭据
    /// </summary>
    Task<bool> ValidateCredentialsAsync(string appUid, string apiKey, string clientSecret);
    
    /// <summary>
    /// 创建新应用
    /// </summary>
    Task<Application> CreateAsync(Application application);
    
    /// <summary>
    /// 更新应用
    /// </summary>
    Task<Application> UpdateAsync(Application application);
    
    /// <summary>
    /// 获取所有应用
    /// </summary>
    Task<IEnumerable<Application>> GetAllAsync();
    
    /// <summary>
    /// 检查应用UID是否已存在
    /// </summary>
    Task<bool> ExistsByAppUidAsync(string appUid);
}