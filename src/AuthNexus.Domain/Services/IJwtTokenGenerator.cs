using AuthNexus.Domain.Models;

namespace AuthNexus.Domain.Services
{
    /// <summary>
    /// JWT令牌生成服务接口
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="roles">角色列表</param>
        /// <param name="permissions">权限列表</param>
        /// <returns>令牌响应，包含访问令牌和刷新令牌</returns>
        TokenResponse GenerateToken(Guid userId, string username, List<string> roles, List<string> permissions);
        
        /// <summary>
        /// 刷新JWT令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>新的令牌响应</returns>
        TokenResponse RefreshToken(string refreshToken);
    }
}