using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 用户身份实体，代表应用中的一个用户
/// </summary>
public class UserIdentity : Entity
{
    /// <summary>
    /// 所属应用的ID
    /// </summary>
    public Guid ApplicationId { get; private set; }
    
    /// <summary>
    /// 外部用户ID，在同一应用内唯一
    /// </summary>
    public string ExternalUserId { get; private set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 导航属性：所属应用
    /// </summary>
    public Application Application { get; private set; }
    
    /// <summary>
    /// 用户拥有的角色列表
    /// </summary>
    private readonly List<UserRoleAssignment> _roles = new();
    public IReadOnlyList<UserRoleAssignment> Roles => _roles.AsReadOnly();
    
    /// <summary>
    /// 用户直接拥有的权限列表
    /// </summary>
    private readonly List<UserDirectPermissionAssignment> _directPermissions = new();
    public IReadOnlyList<UserDirectPermissionAssignment> DirectPermissions => _directPermissions.AsReadOnly();

    // EF Core所需的无参构造函数
    private UserIdentity() { }

    /// <summary>
    /// 创建新的用户身份
    /// </summary>
    public UserIdentity(Guid applicationId, string externalUserId)
    {
        if (applicationId == Guid.Empty)
            throw new ArgumentException("应用ID不能为空", nameof(applicationId));
            
        if (string.IsNullOrWhiteSpace(externalUserId))
            throw new ArgumentException("外部用户ID不能为空", nameof(externalUserId));
        
        ApplicationId = applicationId;
        ExternalUserId = externalUserId;
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 给用户分配角色
    /// </summary>
    public void AssignRole(Role role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));
            
        if (role.ApplicationId != ApplicationId)
            throw new InvalidOperationException("不能将其他应用的角色分配给此用户");
            
        if (_roles.Any(r => r.RoleId == role.Id))
            return; // 已存在此角色，忽略
            
        _roles.Add(new UserRoleAssignment(Id, role.Id));
    }
    
    /// <summary>
    /// 从用户中移除角色
    /// </summary>
    public void RemoveRole(Guid roleId)
    {
        var assignment = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (assignment != null)
        {
            _roles.Remove(assignment);
        }
    }
    
    /// <summary>
    /// 直接给用户分配权限
    /// </summary>
    public void AssignDirectPermission(PermissionDefinition permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));
            
        if (permission.ApplicationId != ApplicationId)
            throw new InvalidOperationException("不能将其他应用的权限分配给此用户");
            
        if (_directPermissions.Any(p => p.PermissionDefinitionId == permission.Id))
            return; // 已存在此权限，忽略
            
        _directPermissions.Add(new UserDirectPermissionAssignment(Id, permission.Id));
    }
    
    /// <summary>
    /// 从用户中移除直接权限
    /// </summary>
    public void RemoveDirectPermission(Guid permissionId)
    {
        var assignment = _directPermissions.FirstOrDefault(p => p.PermissionDefinitionId == permissionId);
        if (assignment != null)
        {
            _directPermissions.Remove(assignment);
        }
    }
}