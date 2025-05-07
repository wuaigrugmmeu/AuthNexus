using AuthNexus.Domain.Repositories;
using AuthNexus.Infrastructure.Data;
using AuthNexus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthNexus.Infrastructure;

/// <summary>
/// 基础设施层服务注册
/// </summary>
public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册数据库上下文
        services.AddDbContext<AuthNexusDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        // 注册仓储
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // 注册数据库初始化服务
        services.AddDbInitializer();

        return services;
    }
}