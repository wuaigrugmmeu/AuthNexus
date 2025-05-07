using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 用户-角色分配关系实体
/// </summary>
public class UserRoleAssignment : Entity
{
    /// <summary>
    /// 用户身份ID
    /// </summary>
    public Guid UserIdentityId { get; private set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }
    
    /// <summary>
    /// 导航属性：用户身份
    /// </summary>
    public UserIdentity UserIdentity { get; private set; }
    
    /// <summary>
    /// 导航属性：角色
    /// </summary>
    public Role Role { get; private set; }
    
    // EF Core所需的无参构造函数
    private UserRoleAssignment() { }
    
    /// <summary>
    /// 创建用户-角色分配关系
    /// </summary>
    public UserRoleAssignment(Guid userIdentityId, Guid roleId)
    {
        if (userIdentityId == Guid.Empty)
            throw new ArgumentException("用户身份ID不能为空", nameof(userIdentityId));
            
        if (roleId == Guid.Empty)
            throw new ArgumentException("角色ID不能为空", nameof(roleId));
            
        UserIdentityId = userIdentityId;
        RoleId = roleId;
    }
}