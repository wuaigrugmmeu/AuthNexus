using System.ComponentModel.DataAnnotations;

namespace AuthNexus.Application.Permissions;

/// <summary>
/// 创建权限定义请求
/// </summary>
public class CreatePermissionRequest
{
    /// <summary>
    /// 应用的唯一标识符
    /// </summary>
    [Required(ErrorMessage = "应用标识不能为空")]
    public string ApplicationId { get; set; }
    
    /// <summary>
    /// 权限代码
    /// </summary>
    [Required(ErrorMessage = "权限代码不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "权限代码长度必须在1-100个字符之间")]
    public string Code { get; set; }
    
    /// <summary>
    /// 权限描述
    /// </summary>
    [StringLength(500, ErrorMessage = "权限描述长度不能超过500个字符")]
    public string Description { get; set; }
}

/// <summary>
/// 更新权限定义请求
/// </summary>
public class UpdatePermissionRequest
{
    /// <summary>
    /// 应用的唯一标识符
    /// </summary>
    [Required(ErrorMessage = "应用标识不能为空")]
    public string ApplicationId { get; set; }
    
    /// <summary>
    /// 权限代码
    /// </summary>
    [Required(ErrorMessage = "权限代码不能为空")]
    public string Code { get; set; }
    
    /// <summary>
    /// 权限描述
    /// </summary>
    [StringLength(500, ErrorMessage = "权限描述长度不能超过500个字符")]
    public string Description { get; set; }
}