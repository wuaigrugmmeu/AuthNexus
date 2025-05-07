using AuthNexus.Application.Applications;
using AuthNexus.Application.Permissions;
using AuthNexus.Application.Roles;
using AuthNexus.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace AuthNexus.Application;

/// <summary>
/// 应用层服务注册
/// </summary>
public static class ApplicationServiceRegistration
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 注册应用服务
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}