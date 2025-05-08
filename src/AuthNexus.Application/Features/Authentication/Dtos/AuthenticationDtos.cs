namespace AuthNexus.Application.Features.Authentication.Dtos
{
    /// <summary>
    /// 用户登录请求DTO
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// 用户名/邮箱
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 令牌响应DTO
    /// </summary>
    public class TokenResponseDto
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;
        
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
        
        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// 令牌类型
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// 用户简档DTO
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
        
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// 用户注册DTO
    /// </summary>
    public class UserRegistrationDto
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
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}