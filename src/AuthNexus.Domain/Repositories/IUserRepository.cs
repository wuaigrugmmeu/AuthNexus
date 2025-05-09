using AuthNexus.Domain.Entities;

namespace AuthNexus.Domain.Repositories;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// 根据ID获取用户身份
    /// </summary>
    Task<UserIdentity> GetByIdAsync(Guid id);
    
    /// <summary>
    /// 根据应用ID和外部用户ID获取用户身份
    /// </summary>
    Task<UserIdentity> GetByExternalIdAsync(Guid applicationId, string externalUserId);
    
    /// <summary>
    /// 创建新用户身份
    /// </summary>
    Task<UserIdentity> CreateAsync(UserIdentity userIdentity);
    
    /// <summary>
    /// 更新用户身份
    /// </summary>
    Task<UserIdentity> UpdateAsync(UserIdentity userIdentity);
    
    /// <summary>
    /// 获取应用的所有用户身份
    /// </summary>
    Task<IEnumerable<UserIdentity>> GetByApplicationIdAsync(Guid applicationId);
    
    /// <summary>
    /// 获取用户身份及其所有角色
    /// </summary>
    Task<UserIdentity> GetWithRolesAsync(Guid id);
    
    /// <summary>
    /// 获取用户身份及其所有直接权限
    /// </summary>
    Task<UserIdentity> GetWithDirectPermissionsAsync(Guid id);
    
    /// <summary>
    /// 获取用户身份及其所有角色和直接权限
    /// </summary>
    Task<UserIdentity> GetWithRolesAndPermissionsAsync(Guid id);
    
    /// <summary>
    /// 检查用户是否拥有指定权限（通过角色或直接授权）
    /// </summary>
    Task<bool> HasPermissionAsync(Guid applicationId, string externalUserId, string permissionCode);
    
    /// <summary>
    /// 获取用户所有权限（包括通过角色获得的权限和直接授权的权限）
    /// </summary>
    Task<IEnumerable<PermissionDefinition>> GetAllPermissionsAsync(Guid applicationId, string externalUserId);

    /// <summary>
    /// 根据用户名或邮箱获取用户
    /// </summary>
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> ExistsByUsernameAsync(string username);

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// 获取用户的角色
    /// </summary>
    Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId);

    /// <summary>
    /// 获取用户的权限
    /// </summary>
    Task<IReadOnlyList<Permission>> GetUserPermissionsAsync(Guid userId);

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    Task AssignRoleToUserAsync(Guid userId, Guid roleId);

    /// <summary>
    /// 移除用户的角色
    /// </summary>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}