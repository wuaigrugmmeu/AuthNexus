using System.Security.Claims;
using AuthNexus.SharedKernel.Constants;
using AuthNexus.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AuthNexus.Infrastructure.Authentication
{
    /// <summary>
    /// 当前用户服务实现
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        public Guid? UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(CustomClaimTypes.UserId);
                return !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : null;
            }
        }

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        public string? Username
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirstValue(CustomClaimTypes.UserName);
            }
        }

        /// <summary>
        /// 检查当前用户是否拥有指定权限
        /// </summary>
        public bool HasPermission(string permission)
        {
            return _httpContextAccessor.HttpContext?.User
                .FindAll(CustomClaimTypes.Permission)
                .Any(c => c.Value == permission) ?? false;
        }

        /// <summary>
        /// 获取当前用户所有角色
        /// </summary>
        public IEnumerable<string> GetRoles()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindAll(CustomClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// 获取当前用户所有权限
        /// </summary>
        public IEnumerable<string> GetPermissions()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindAll(CustomClaimTypes.Permission)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }
}