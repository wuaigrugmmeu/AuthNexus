using AuthNexus.Application.Common;
using AuthNexus.Application.Permissions;

namespace AuthNexus.Application.Users;

/// <summary>
/// 用户管理服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    Task<ResultDto<UserDto>> GetUserAsync(string appUid, string externalUserId);
    
    /// <summary>
    /// 为用户分配角色
    /// </summary>
    Task<ResultDto<UserDto>> AssignRolesToUserAsync(string appUid, string externalUserId, AssignRolesToUserRequest request);
    
    /// <summary>
    /// 从用户移除角色
    /// </summary>
    Task<ResultDto<UserDto>> RemoveRolesFromUserAsync(string appUid, string externalUserId, RemoveRolesFromUserRequest request);
    
    /// <summary>
    /// 为用户分配直接权限
    /// </summary>
    Task<ResultDto<UserDto>> AssignDirectPermissionsToUserAsync(string appUid, string externalUserId, AssignDirectPermissionsToUserRequest request);
    
    /// <summary>
    /// 从用户移除直接权限
    /// </summary>
    Task<ResultDto<UserDto>> RemoveDirectPermissionsFromUserAsync(string appUid, string externalUserId, RemoveDirectPermissionsFromUserRequest request);
    
    /// <summary>
    /// 获取用户的所有权限
    /// </summary>
    Task<ResultDto<IEnumerable<PermissionDefinitionDto>>> GetUserPermissionsAsync(string appUid, string externalUserId);
    
    /// <summary>
    /// 校验用户权限
    /// </summary>
    Task<ResultDto<bool>> CheckPermissionAsync(CheckPermissionRequest request);

    /// <summary>
    /// 校验用户权限
    /// </summary>
    Task<ResultDto<bool>> CheckPermissionAsync(string appUid, string externalUserId, string permissionCode);
}