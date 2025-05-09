using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 角色实体，代表应用中的一个用户角色
/// </summary>
public class Role : Entity
{
    /// <summary>
    /// 所属应用的ID
    /// </summary>
    public Guid ApplicationId { get; private set; }
    
    /// <summary>
    /// 角色名称，在同一应用内唯一
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }
    
    /// <summary>
    /// 导航属性：所属应用
    /// </summary>
    public Application Application { get; private set; }
    
    /// <summary>
    /// 角色所拥有的权限列表
    /// </summary>
    private readonly List<RolePermissionAssignment> _permissions = new();
    public IReadOnlyList<RolePermissionAssignment> Permissions => _permissions.AsReadOnly();
    
    /// <summary>
    /// 与RolePermission实体的导航关系
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    // EF Core所需的无参构造函数
    private Role() { }

    /// <summary>
    /// 创建新的角色
    /// </summary>
    public Role(Guid applicationId, string name, string description)
    {
        if (applicationId == Guid.Empty)
            throw new ArgumentException("应用ID不能为空", nameof(applicationId));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("角色名称不能为空", nameof(name));
        
        ApplicationId = applicationId;
        Name = name;
        Description = description ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("角色名称不能为空", nameof(name));
            
        Name = name;
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 添加权限到角色
    /// </summary>
    public void AddPermission(PermissionDefinition permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));
            
        if (permission.ApplicationId != ApplicationId)
            throw new InvalidOperationException("不能将其他应用的权限分配给此角色");
            
        if (_permissions.Any(p => p.PermissionDefinitionId == permission.Id))
            return; // 已存在此权限，忽略
            
        _permissions.Add(new RolePermissionAssignment(Id, permission.Id));
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 从角色中移除权限
    /// </summary>
    public void RemovePermission(Guid permissionId)
    {
        var assignment = _permissions.FirstOrDefault(p => p.PermissionDefinitionId == permissionId);
        if (assignment != null)
        {
            _permissions.Remove(assignment);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}