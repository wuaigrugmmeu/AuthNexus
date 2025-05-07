using System.ComponentModel.DataAnnotations;

namespace AuthNexus.Application.Applications;

/// <summary>
/// 应用注册请求
/// </summary>
public class RegisterApplicationRequest
{
    /// <summary>
    /// 应用的唯一标识符，用于外部引用
    /// </summary>
    [Required(ErrorMessage = "应用标识不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "应用标识长度必须在3-50个字符之间")]
    public string AppUID { get; set; }
    
    /// <summary>
    /// 应用名称
    /// </summary>
    [Required(ErrorMessage = "应用名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "应用名称长度必须在1-100个字符之间")]
    public string Name { get; set; }
    
    /// <summary>
    /// 应用描述
    /// </summary>
    [StringLength(500, ErrorMessage = "应用描述长度不能超过500个字符")]
    public string Description { get; set; }
}

/// <summary>
/// 应用更新请求
/// </summary>
public class UpdateApplicationRequest
{
    /// <summary>
    /// 应用ID
    /// </summary>
    [Required(ErrorMessage = "应用ID不能为空")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// 应用名称
    /// </summary>
    [Required(ErrorMessage = "应用名称不能为空")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "应用名称长度必须在1-100个字符之间")]
    public string Name { get; set; }
    
    /// <summary>
    /// 应用描述
    /// </summary>
    [StringLength(500, ErrorMessage = "应用描述长度不能超过500个字符")]
    public string Description { get; set; }
}