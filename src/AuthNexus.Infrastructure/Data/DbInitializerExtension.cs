using AuthNexus.Infrastructure.Data.Seed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthNexus.Infrastructure.Data;

/// <summary>
/// 数据库初始化扩展
/// </summary>
public static class DbInitializerExtension
{
    /// <summary>
    /// 添加数据库初始化服务
    /// </summary>
    public static IServiceCollection AddDbInitializer(this IServiceCollection services)
    {
        services.AddScoped<DbInitializer>();
        return services;
    }

    /// <summary>
    /// 执行数据库初始化
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

        try
        {
            logger.LogInformation("开始初始化数据库...");
            await initializer.InitializeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "初始化数据库时发生错误");
        }
    }
}