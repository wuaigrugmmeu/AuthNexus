using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System;
using System.Linq;

namespace AuthNexus.Infrastructure.Authorization
{
    /// <summary>
    /// 授权策略修复扩展 - 添加更宽松和详细的授权策略实现
    /// </summary>
    public static class AuthorizationPolicyFixExtensions
    {
        /// <summary>
        /// 添加修复的授权策略
        /// </summary>
        public static IServiceCollection FixPermissionAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // 修复管理员角色授权策略 - 使用不区分大小写的比较
                options.AddPolicy(PolicyNames.RequireAdminRole, policy =>
                    policy.RequireAssertion(context => 
                    {
                        var userRoles = context.User.FindAll(ClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .Union(context.User.FindAll(CustomClaimTypes.Role)
                                  .Select(x => x.Value.ToLowerInvariant()))
                            .ToList();
                        
                        // 输出日志以便调试
                        Console.WriteLine($"用户所有角色 (不区分大小写): {string.Join(", ", userRoles)}");
                        
                        // 使用不区分大小写的比较
                        return userRoles.Contains("admin") || 
                               userRoles.Contains("super-admin") || 
                               userRoles.Contains("administrator");
                    }));
                
                // 修复权限检查策略
                options.AddPolicy(PolicyNames.RequirePermission, policy =>
                    policy.RequireAssertion(context => 
                    {
                        // 如果用户是管理员，自动授予所有权限
                        var userRoles = context.User.FindAll(ClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .Union(context.User.FindAll(CustomClaimTypes.Role)
                                  .Select(x => x.Value.ToLowerInvariant()))
                            .ToList();
                        
                        if (userRoles.Contains("admin") || 
                            userRoles.Contains("super-admin") || 
                            userRoles.Contains("administrator"))
                        {
                            Console.WriteLine("管理员角色自动授予所有权限");
                            return true;
                        }
                        
                        // 否则检查特定权限
                        var permissions = context.User.FindAll(CustomClaimTypes.Permission)
                            .Select(x => x.Value)
                            .ToList();
                        
                        Console.WriteLine($"用户特定权限: {string.Join(", ", permissions)}");
                        
                        return permissions.Contains(PermissionConstants.ViewPermissions) || 
                               permissions.Contains(PermissionConstants.ViewApplications) || 
                               permissions.Contains(PermissionConstants.ViewRoles) || 
                               permissions.Contains(PermissionConstants.ViewUsers);
                    }));
            });

            return services;
        }
    }
}
