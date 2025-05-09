using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AuthNexus.Infrastructure.Authorization
{
    /// <summary>
    /// 权限要求 - 用于基于权限的授权
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    /// <summary>
    /// 权限授权处理程序 - 检查用户是否具有所需权限
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            // 获取用户声明中的所有权限
            var permissions = context.User
                .FindAll(CustomClaimTypes.Permission)
                .Select(x => x.Value)
                .ToList();
                
            // 获取用户角色
            var roles = context.User.FindAll(CustomClaimTypes.Role)
                .Union(context.User.FindAll(ClaimTypes.Role))
                .Select(x => x.Value.ToLowerInvariant())
                .ToList();

            // 调试输出
            Console.WriteLine($"检查权限: {requirement.Permission}");
            Console.WriteLine($"用户权限: {string.Join(", ", permissions)}");
            Console.WriteLine($"用户角色: {string.Join(", ", roles)}");
            
            // 管理员角色自动通过权限检查
            if (roles.Contains("admin") || roles.Contains("super-admin"))
            {
                Console.WriteLine("用户具有管理员角色，自动授予权限");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 检查用户是否具有所需权限
            if (permissions.Contains(requirement.Permission))
            {
                Console.WriteLine($"用户具有权限: {requirement.Permission}");
                context.Succeed(requirement);
            }
            else
            {
                Console.WriteLine($"权限验证失败: {requirement.Permission}");
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 授权策略提供者 - 动态创建授权策略
    /// </summary>
    public static class AuthorizationPolicyProviderExtensions
    {
        /// <summary>
        /// 添加权限授权策略
        /// </summary>
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            // 注册权限授权处理程序
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            // 配置授权策略
            services.AddAuthorization(options =>
            {
                // 添加基于角色的策略，同时支持Admin和super-admin角色
                options.AddPolicy(PolicyNames.RequireAdminRole, policy =>
                    policy.RequireAssertion(context => {
                        var roles = context.User.FindAll(CustomClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .ToList();
                        
                        var standardRoles = context.User.FindAll(ClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .ToList();
                        
                        // 合并两种类型的角色声明，并转换为小写进行比较
                        var allRoles = roles.Union(standardRoles).ToList();
                        Console.WriteLine($"用户角色: {string.Join(", ", allRoles)}");
                        
                        return allRoles.Contains("admin") || 
                               allRoles.Contains("super-admin") || 
                               allRoles.Contains("administrator");
                    }));
                
                options.AddPolicy(PolicyNames.RequireUserRole, policy =>
                    policy.RequireAssertion(context => {
                        var roles = context.User.FindAll(CustomClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .ToList();
                        
                        var standardRoles = context.User.FindAll(ClaimTypes.Role)
                            .Select(x => x.Value.ToLowerInvariant())
                            .ToList();
                        
                        var allRoles = roles.Union(standardRoles).ToList();
                        return allRoles.Contains("user") || allRoles.Contains("viewer");
                    }));
                
                // 添加基于权限的策略
                options.AddPolicy(PolicyNames.RequirePermission, policy =>
                    policy.RequireAssertion(context => 
                    {
                        var permissions = context.User.FindAll(CustomClaimTypes.Permission)
                            .Select(x => x.Value)
                            .ToList();
                        
                        // 检查是否有管理员角色，如果是管理员则自动通过权限检查
                        var roles = context.User.FindAll(CustomClaimTypes.Role)
                            .Union(context.User.FindAll(ClaimTypes.Role))
                            .Select(x => x.Value.ToLowerInvariant())
                            .ToList();
                        
                        // 管理员角色自动拥有所有权限
                        if (roles.Contains("admin") || roles.Contains("super-admin"))
                        {
                            return true;
                        }
                        
                        // 检查用户是否拥有任一所需权限（使用OR逻辑而非AND逻辑）
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