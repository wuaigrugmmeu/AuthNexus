namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 用户-角色关联实体
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }
        
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
        /// 导航属性：角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;
        
        public UserRole()
        {
            AssignedAt = DateTime.UtcNow;
        }
    }
}