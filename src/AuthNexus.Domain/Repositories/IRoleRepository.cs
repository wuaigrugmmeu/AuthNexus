using AuthNexus.Domain.Entities;

namespace AuthNexus.Domain.Repositories;

/// <summary>
/// 角色仓储接口
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    Task<Role> GetByIdAsync(Guid id);
    
    /// <summary>
    /// 根据应用ID和角色名称获取角色
    /// </summary>
    Task<Role> GetByNameAsync(Guid applicationId, string name);
    
    /// <summary>
    /// 获取应用的所有角色
    /// </summary>
    Task<IEnumerable<Role>> GetByApplicationIdAsync(Guid applicationId);
    
    /// <summary>
    /// 创建新角色
    /// </summary>
    Task<Role> CreateAsync(Role role);
    
    /// <summary>
    /// 更新角色
    /// </summary>
    Task<Role> UpdateAsync(Role role);
    
    /// <summary>
    /// 删除角色
    /// </summary>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// 检查应用中是否存在指定名称的角色
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid applicationId, string name);
    
    /// <summary>
    /// 获取角色及其所有权限
    /// </summary>
    Task<Role> GetWithPermissionsAsync(Guid id);
    
    /// <summary>
    /// 根据ID列表获取多个角色
    /// </summary>
    Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids);
}