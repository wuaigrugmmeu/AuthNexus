using System.ComponentModel.DataAnnotations;

namespace AuthNexus.Application.Roles;

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "角色名称长度必须在1-100个字符之间")]
    public string Name { get; set; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    [StringLength(500, ErrorMessage = "角色描述长度不能超过500个字符")]
    public string Description { get; set; }
}

/// <summary>
/// 更新角色请求
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "角色名称长度必须在1-100个字符之间")]
    public string Name { get; set; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    [StringLength(500, ErrorMessage = "角色描述长度不能超过500个字符")]
    public string Description { get; set; }
}

/// <summary>
/// 角色权限分配请求
/// </summary>
public class AssignPermissionsToRoleRequest
{
    /// <summary>
    /// 权限代码列表
    /// </summary>
    [Required(ErrorMessage = "权限代码列表不能为空")]
    public IEnumerable<string> PermissionCodes { get; set; }
}

/// <summary>
/// 从角色移除权限请求
/// </summary>
public class RemovePermissionsFromRoleRequest
{
    /// <summary>
    /// 权限代码列表
    /// </summary>
    [Required(ErrorMessage = "权限代码列表不能为空")]
    public IEnumerable<string> PermissionCodes { get; set; }
}