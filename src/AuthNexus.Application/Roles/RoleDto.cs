namespace AuthNexus.Application.Roles;

/// <summary>
/// 角色DTO，用于返回角色信息
/// </summary>
public class RoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 所属应用ID
    /// </summary>
    public Guid ApplicationId { get; set; }
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 角色描述
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
    
    /// <summary>
    /// 角色拥有的权限ID列表
    /// </summary>
    public IEnumerable<Guid> PermissionIds { get; set; } = Array.Empty<Guid>();
}