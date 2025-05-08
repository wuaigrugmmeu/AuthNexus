namespace AuthNexus.Domain.Services
{
    /// <summary>
    /// 密码哈希服务接口
    /// </summary>
    public interface IPasswordHashingService
    {
        /// <summary>
        /// 对密码进行哈希处理
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <returns>哈希后的密码</returns>
        string HashPassword(string password);
        
        /// <summary>
        /// 验证密码是否匹配
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <param name="passwordHash">哈希后的密码</param>
        /// <returns>是否匹配</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}