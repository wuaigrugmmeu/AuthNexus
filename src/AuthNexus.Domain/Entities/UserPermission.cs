namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 用户-权限关联实体（直接授权）
    /// </summary>
    public class UserPermission
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        
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
        /// 导航属性：用户
        /// </summary>
        public virtual User User { get; set; } = null!;
        
        /// <summary>
        /// 导航属性：权限
        /// </summary>
        public virtual Permission Permission { get; set; } = null!;
        
        public UserPermission()
        {
            AssignedAt = DateTime.UtcNow;
        }
    }
}