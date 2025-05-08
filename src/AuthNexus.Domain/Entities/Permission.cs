namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 权限实体
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// 权限名称（唯一）
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 权限描述
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// 权限分组
        /// </summary>
        public string? Group { get; set; }
        
        /// <summary>
        /// 是否是系统权限
        /// </summary>
        public bool IsSystem { get; set; } = false;
        
        /// <summary>
        /// 与角色的关联
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        
        /// <summary>
        /// 与用户的直接关联（直接授权）
        /// </summary>
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}