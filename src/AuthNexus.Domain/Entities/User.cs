namespace AuthNexus.Domain.Entities
{
    /// <summary>
    /// 用户状态枚举
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 激活状态
        /// </summary>
        Active = 0,
        
        /// <summary>
        /// 未验证状态
        /// </summary>
        Unverified = 1,
        
        /// <summary>
        /// 已锁定状态
        /// </summary>
        Locked = 2,
        
        /// <summary>
        /// 已禁用状态
        /// </summary>
        Disabled = 3
    }

    /// <summary>
    /// 用户实体
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码哈希
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// 昵称/显示名
        /// </summary>
        public string? DisplayName { get; set; }
        
        /// <summary>
        /// 电话号码
        /// </summary>
        public string? PhoneNumber { get; set; }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Active;
        
        /// <summary>
        /// 是否是活跃用户
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
        
        /// <summary>
        /// 上次密码修改时间
        /// </summary>
        public DateTime? LastPasswordChangedAt { get; set; }
        
        /// <summary>
        /// 登录失败次数
        /// </summary>
        public int AccessFailedCount { get; set; } = 0;
        
        /// <summary>
        /// 锁定结束时间
        /// </summary>
        public DateTime? LockoutEnd { get; set; }
        
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string? RefreshToken { get; set; }
        
        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        /// <summary>
        /// 用户角色关联
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        
        /// <summary>
        /// 用户权限关联（直接分配的权限）
        /// </summary>
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}