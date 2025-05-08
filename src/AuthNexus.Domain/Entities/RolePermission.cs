namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 角色-权限关联实体
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }
        
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PermissionId { get; set; }
        
        /// <summary>
        /// 分配时间
        /// </summary>
        public DateTime AssignedAt { get; set; }
        
        /// <summary>
        /// 分配人ID
        /// </summary>
        public Guid? AssignedBy { get; set; }
        
        /// <summary>
        /// 导航属性：角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;
        
        /// <summary>
        /// 导航属性：权限
        /// </summary>
        public virtual Permission Permission { get; set; } = null!;
        
        public RolePermission()
        {
            AssignedAt = DateTime.UtcNow;
        }
    }
}