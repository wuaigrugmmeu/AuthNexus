using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthNexus.SharedKernel.Interfaces;
using AuthNexus.Application.Common;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using AuthNexus.SharedKernel.Constants;

namespace AuthNexus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiagnosticsController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public DiagnosticsController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("my-auth-info")]
        public ActionResult<object> GetMyAuthInfo()
        {
            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username;
            var roles = _currentUserService.GetRoles().ToList();
            var permissions = _currentUserService.GetPermissions().ToList();
            
            // 获取原始声明，提供更准确的信息
            var claims = HttpContext.User.Claims
                .Select(c => new { Type = c.Type, Value = c.Value })
                .ToList();
            
            // 检查管理员角色
            bool isAdmin = roles.Any(r => 
                r.Equals("admin", System.StringComparison.OrdinalIgnoreCase) || 
                r.Equals("super-admin", System.StringComparison.OrdinalIgnoreCase) || 
                r.Equals("administrator", System.StringComparison.OrdinalIgnoreCase));
            
            // 构造返回结果
            var result = new
            {
                UserId = userId,
                Username = username,
                Roles = roles,
                Permissions = permissions,
                Claims = claims,
                IsAdmin = isAdmin,
                AuthenticationType = HttpContext.User.Identity?.AuthenticationType,
                IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false
            };
            
            return Ok(result);
        }
        
        [HttpGet("check-permission/{permissionCode}")]
        public ActionResult<object> CheckPermission(string permissionCode)
        {
            bool hasPermission = _currentUserService.HasPermission(permissionCode);
            
            var result = new
            {
                Permission = permissionCode,
                HasPermission = hasPermission,
                Roles = _currentUserService.GetRoles().ToList(),
                UserPermissions = _currentUserService.GetPermissions().ToList()
            };
            
            return Ok(result);
        }
        
        [HttpGet("check-admin-role")]
        public ActionResult<object> CheckAdminRole()
        {
            var roles = _currentUserService.GetRoles().ToList();
            var customRoles = HttpContext.User.FindAll(CustomClaimTypes.Role).Select(c => c.Value).ToList();
            var standardRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            
            bool isAdmin = roles.Any(r => 
                r.Equals("admin", System.StringComparison.OrdinalIgnoreCase) || 
                r.Equals("super-admin", System.StringComparison.OrdinalIgnoreCase) || 
                r.Equals("administrator", System.StringComparison.OrdinalIgnoreCase));
            
            var result = new
            {
                IsAdmin = isAdmin,
                AllRoles = roles,
                CustomRolesClaims = customRoles,
                StandardRolesClaims = standardRoles
            };
            
            return Ok(result);
        }
    }
}
