using AuthNexus.Application.Common;

namespace AuthNexus.Application.Roles;

/// <summary>
/// 角色管理服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 创建角色
    /// </summary>
    Task<ResultDto<RoleDto>> CreateRoleAsync(string appUid, CreateRoleRequest request);
    
    /// <summary>
    /// 获取角色信息
    /// </summary>
    Task<ResultDto<RoleDto>> GetRoleAsync(string appUid, string roleName);
    
    /// <summary>
    /// 更新角色
    /// </summary>
    Task<ResultDto<RoleDto>> UpdateRoleAsync(string appUid, string roleName, UpdateRoleRequest request);
    
    /// <summary>
    /// 删除角色
    /// </summary>
    Task<ResultDto> DeleteRoleAsync(string appUid, string roleName);
    
    /// <summary>
    /// 获取应用的所有角色
    /// </summary>
    Task<ResultDto<IEnumerable<RoleDto>>> GetApplicationRolesAsync(string appUid);
    
    /// <summary>
    /// 为角色分配权限
    /// </summary>
    Task<ResultDto<RoleDto>> AssignPermissionsToRoleAsync(string appUid, string roleName, AssignPermissionsToRoleRequest request);
    
    /// <summary>
    /// 从角色移除权限
    /// </summary>
    Task<ResultDto<RoleDto>> RemovePermissionsFromRoleAsync(string appUid, string roleName, RemovePermissionsFromRoleRequest request);
}