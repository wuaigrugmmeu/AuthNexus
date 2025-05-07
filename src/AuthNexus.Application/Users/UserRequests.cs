using System.ComponentModel.DataAnnotations;

namespace AuthNexus.Application.Users;

/// <summary>
/// 用户角色分配请求
/// </summary>
public class AssignRolesToUserRequest
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [Required(ErrorMessage = "应用ID不能为空")]
    public Guid ApplicationId { get; set; }
    
    /// <summary>
    /// 外部用户ID
    /// </summary>
    [Required(ErrorMessage = "外部用户ID不能为空")]
    public string ExternalUserId { get; set; }
    
    /// <summary>
    /// 角色名称列表
    /// </summary>
    [Required(ErrorMessage = "角色名称列表不能为空")]
    public IEnumerable<string> RoleNames { get; set; }
}

/// <summary>
/// 从用户移除角色请求
/// </summary>
public class RemoveRolesFromUserRequest
{
    /// <summary>
    /// 角色名称列表
    /// </summary>
    [Required(ErrorMessage = "角色名称列表不能为空")]
    public IEnumerable<string> RoleNames { get; set; }
}

/// <summary>
/// 用户直接权限分配请求
/// </summary>
public class AssignDirectPermissionsToUserRequest
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [Required(ErrorMessage = "应用ID不能为空")]
    public Guid ApplicationId { get; set; }
    
    /// <summary>
    /// 外部用户ID
    /// </summary>
    [Required(ErrorMessage = "外部用户ID不能为空")]
    public string ExternalUserId { get; set; }
    
    /// <summary>
    /// 权限代码列表
    /// </summary>
    [Required(ErrorMessage = "权限代码列表不能为空")]
    public IEnumerable<string> PermissionCodes { get; set; }
}

/// <summary>
/// 从用户移除直接权限请求
/// </summary>
public class RemoveDirectPermissionsFromUserRequest
{
    /// <summary>
    /// 权限代码列表
    /// </summary>
    [Required(ErrorMessage = "权限代码列表不能为空")]
    public IEnumerable<string> PermissionCodes { get; set; }
}

/// <summary>
/// 权限校验请求
/// </summary>
public class CheckPermissionRequest
{
    /// <summary>
    /// 应用的唯一标识符
    /// </summary>
    [Required(ErrorMessage = "应用标识不能为空")]
    public string AppUID { get; set; }
    
    /// <summary>
    /// 外部用户ID
    /// </summary>
    [Required(ErrorMessage = "用户ID不能为空")]
    public string ExternalUserId { get; set; }
    
    /// <summary>
    /// 权限代码
    /// </summary>
    [Required(ErrorMessage = "权限代码不能为空")]
    public string PermissionCode { get; set; }
}