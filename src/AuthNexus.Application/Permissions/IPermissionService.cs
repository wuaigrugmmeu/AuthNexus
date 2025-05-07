using AuthNexus.Application.Common;

namespace AuthNexus.Application.Permissions;

/// <summary>
/// 权限管理服务接口
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// 创建权限定义
    /// </summary>
    Task<ResultDto<PermissionDefinitionDto>> CreatePermissionAsync(string appUid, CreatePermissionRequest request);
    
    /// <summary>
    /// 获取权限定义信息
    /// </summary>
    Task<ResultDto<PermissionDefinitionDto>> GetPermissionAsync(string appUid, string code);
    
    /// <summary>
    /// 更新权限定义
    /// </summary>
    Task<ResultDto<PermissionDefinitionDto>> UpdatePermissionAsync(string appUid, string code, UpdatePermissionRequest request);
    
    /// <summary>
    /// 删除权限定义
    /// </summary>
    Task<ResultDto> DeletePermissionAsync(string appUid, string code);
    
    /// <summary>
    /// 获取应用的所有权限定义
    /// </summary>
    Task<ResultDto<IEnumerable<PermissionDefinitionDto>>> GetApplicationPermissionsAsync(string appUid);
}