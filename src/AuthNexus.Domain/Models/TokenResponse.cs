namespace AuthNexus.Domain.Models
{
    /// <summary>
    /// JWT令牌响应模型
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 访问令牌过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}