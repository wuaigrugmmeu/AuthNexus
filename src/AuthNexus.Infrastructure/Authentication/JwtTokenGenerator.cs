using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthNexus.Domain.Models;
using AuthNexus.Domain.Services;
using AuthNexus.SharedKernel.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthNexus.Infrastructure.Authentication
{
    /// <summary>
    /// JWT令牌生成服务实现
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        public TokenResponse GenerateToken(Guid userId, string username, List<string> roles, List<string> permissions)
        {
            // 从配置中获取JWT设置
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var expiryMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpiryMinutes"]);
            var refreshTokenExpiryDays = Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpiryDays"]);

            // 创建声明
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CustomClaimTypes.UserId, userId.ToString()),
                new Claim(CustomClaimTypes.UserName, username)
            };

            // 添加角色声明
            foreach (var role in roles)
            {
                claims.Add(new Claim(CustomClaimTypes.Role, role));
            }

            // 添加权限声明
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permission, permission));
            }

            // 创建密钥和证书
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 计算过期时间
            var expiry = DateTime.UtcNow.AddMinutes(expiryMinutes);
            
            // 创建令牌
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            // 生成刷新令牌
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            // 创建并返回令牌响应
            return new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                ExpiresIn = expiryMinutes * 60 // 转换为秒
            };
        }

        /// <summary>
        /// 刷新JWT令牌
        /// </summary>
        public TokenResponse RefreshToken(string refreshToken)
        {
            // 这里应该验证刷新令牌并从数据库获取用户信息
            // 出于简化，此处仅返回错误
            throw new NotImplementedException("刷新令牌功能尚未实现");
        }

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}