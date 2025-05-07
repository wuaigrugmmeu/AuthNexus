using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 用户-直接权限分配关系实体
/// </summary>
public class UserDirectPermissionAssignment : Entity
{
    /// <summary>
    /// 用户身份ID
    /// </summary>
    public Guid UserIdentityId { get; private set; }
    
    /// <summary>
    /// 权限定义ID
    /// </summary>
    public Guid PermissionDefinitionId { get; private set; }
    
    /// <summary>
    /// 导航属性：用户身份
    /// </summary>
    public UserIdentity UserIdentity { get; private set; }
    
    /// <summary>
    /// 导航属性：权限定义
    /// </summary>
    public PermissionDefinition PermissionDefinition { get; private set; }
    
    // EF Core所需的无参构造函数
    private UserDirectPermissionAssignment() { }
    
    /// <summary>
    /// 创建用户-直接权限分配关系
    /// </summary>
    public UserDirectPermissionAssignment(Guid userIdentityId, Guid permissionDefinitionId)
    {
        if (userIdentityId == Guid.Empty)
            throw new ArgumentException("用户身份ID不能为空", nameof(userIdentityId));
            
        if (permissionDefinitionId == Guid.Empty)
            throw new ArgumentException("权限定义ID不能为空", nameof(permissionDefinitionId));
            
        UserIdentityId = userIdentityId;
        PermissionDefinitionId = permissionDefinitionId;
    }
}