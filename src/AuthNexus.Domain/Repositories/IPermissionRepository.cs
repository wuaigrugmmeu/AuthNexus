using AuthNexus.Domain.Entities;

namespace AuthNexus.Domain.Repositories;

/// <summary>
/// 权限定义仓储接口
/// </summary>
public interface IPermissionRepository
{
    /// <summary>
    /// 根据ID获取权限定义
    /// </summary>
    Task<PermissionDefinition> GetByIdAsync(Guid id);
    
    /// <summary>
    /// 根据应用ID和权限代码获取权限定义
    /// </summary>
    Task<PermissionDefinition> GetByCodeAsync(Guid applicationId, string code);
    
    /// <summary>
    /// 获取应用的所有权限定义
    /// </summary>
    Task<IEnumerable<PermissionDefinition>> GetByApplicationIdAsync(Guid applicationId);
    
    /// <summary>
    /// 创建新权限定义
    /// </summary>
    Task<PermissionDefinition> CreateAsync(PermissionDefinition permission);
    
    /// <summary>
    /// 更新权限定义
    /// </summary>
    Task<PermissionDefinition> UpdateAsync(PermissionDefinition permission);
    
    /// <summary>
    /// 删除权限定义
    /// </summary>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// 检查应用中是否存在指定代码的权限
    /// </summary>
    Task<bool> ExistsByCodeAsync(Guid applicationId, string code);
    
    /// <summary>
    /// 根据ID列表获取多个权限定义
    /// </summary>
    Task<IEnumerable<PermissionDefinition>> GetByIdsAsync(IEnumerable<Guid> ids);
}