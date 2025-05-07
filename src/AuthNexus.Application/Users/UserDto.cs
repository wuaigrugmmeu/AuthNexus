namespace AuthNexus.Application.Users;

/// <summary>
/// 用户DTO，用于返回用户信息
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 所属应用ID
    /// </summary>
    public Guid ApplicationId { get; set; }
    
    /// <summary>
    /// 外部用户ID
    /// </summary>
    public string ExternalUserId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 用户拥有的角色ID列表
    /// </summary>
    public IEnumerable<Guid> RoleIds { get; set; } = Array.Empty<Guid>();
    
    /// <summary>
    /// 用户直接拥有的权限ID列表
    /// </summary>
    public IEnumerable<Guid> DirectPermissionIds { get; set; } = Array.Empty<Guid>();
}