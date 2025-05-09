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
                if (string.IsNullOrEmpty(userId))
                {
                    // 尝试从标准的 sub 声明中获取
                    userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                }
                
                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var id))
                {
                    return id;
                }
                
                return null;
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
            if (_httpContextAccessor.HttpContext?.User == null)
                return false;
                
            // 输出当前用户的所有声明，用于调试
            var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();
            Console.WriteLine($"Current user has {claims.Count} claims");
            foreach (var claim in claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
            }
            
            return _httpContextAccessor.HttpContext.User
                .FindAll(CustomClaimTypes.Permission)
                .Any(c => c.Value == permission);
        }

        /// <summary>
        /// 获取当前用户所有角色
        /// </summary>
        public IEnumerable<string> GetRoles()
        {
            var customRoles = _httpContextAccessor.HttpContext?.User
                .FindAll(CustomClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
                
            var standardRoles = _httpContextAccessor.HttpContext?.User
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>();
                
            return customRoles.Union(standardRoles).Distinct();
        }

        /// <summary>
        /// 获取当前用户所有权限
        /// </summary>
        public IEnumerable<string> GetPermissions()
        {
            if (_httpContextAccessor.HttpContext?.User == null)
            {
                Console.WriteLine("HttpContext.User is null in GetPermissions");
                return Enumerable.Empty<string>();
            }
            
            var permissions = _httpContextAccessor.HttpContext.User
                .FindAll(CustomClaimTypes.Permission)
                .Select(c => c.Value).ToList();
                
            Console.WriteLine($"Found {permissions.Count} permissions for current user");
            return permissions;
        }
    }
}