using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 角色-权限分配关系实体
/// </summary>
public class RolePermissionAssignment : Entity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }
    
    /// <summary>
    /// 权限定义ID
    /// </summary>
    public Guid PermissionDefinitionId { get; private set; }
    
    /// <summary>
    /// 导航属性：角色
    /// </summary>
    public Role Role { get; private set; }
    
    /// <summary>
    /// 导航属性：权限定义
    /// </summary>
    public PermissionDefinition PermissionDefinition { get; private set; }
    
    // EF Core所需的无参构造函数
    private RolePermissionAssignment() { }
    
    /// <summary>
    /// 创建角色-权限分配关系
    /// </summary>
    public RolePermissionAssignment(Guid roleId, Guid permissionDefinitionId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
            
        if (permissionDefinitionId == Guid.Empty)
            throw new ArgumentException("权限定义ID不能为空", nameof(permissionDefinitionId));
            
        RoleId = roleId;
        PermissionDefinitionId = permissionDefinitionId;
    }
}