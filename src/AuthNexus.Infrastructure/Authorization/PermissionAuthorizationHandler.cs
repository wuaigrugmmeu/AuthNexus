using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

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

            // 检查用户是否具有所需权限
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
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
                        var roles = context.User.FindAll(CustomClaimTypes.Role).Select(x => x.Value).ToList();
                        
                        // 输出日志以便调试
                        Console.WriteLine($"用户角色: {string.Join(", ", roles)}");
                        
                        return roles.Contains("Admin") || 
                               roles.Contains("super-admin") || 
                               roles.Contains("admin");
                    }));
                
                options.AddPolicy(PolicyNames.RequireUserRole, policy =>
                    policy.RequireAssertion(context => {
                        var roles = context.User.FindAll(CustomClaimTypes.Role).Select(x => x.Value).ToList();
                        return roles.Contains("User") || roles.Contains("viewer");
                    }));
                
                // 添加基于权限的策略，使用工厂模式动态创建
                // 修改策略配置为至少满足一个权限要求
                options.AddPolicy(PolicyNames.RequirePermission, policy =>
                    policy.RequireAssertion(context => 
                    {
                        var permissions = context.User.FindAll(CustomClaimTypes.Permission).Select(x => x.Value).ToList();
                        
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