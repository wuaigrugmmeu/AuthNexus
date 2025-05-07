namespace AuthNexus.Application.Permissions;

/// <summary>
/// 权限定义DTO，用于返回权限信息
/// </summary>
public class PermissionDefinitionDto
{
    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 所属应用ID
    /// </summary>
    public Guid ApplicationId { get; set; }
    
    /// <summary>
    /// 权限代码
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// 权限描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}