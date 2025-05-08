using AuthNexus.Domain.Repositories;
using AuthNexus.Domain.Services;
using AuthNexus.Infrastructure.Authentication;
using AuthNexus.Infrastructure.Authorization;
using AuthNexus.Infrastructure.Data;
using AuthNexus.Infrastructure.Repositories;
using AuthNexus.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AuthNexus.Infrastructure
{
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
            // 添加DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // 添加仓储
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // 添加认证服务
            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            
            // 添加HttpContextAccessor
            services.AddHttpContextAccessor();
            
            // 添加当前用户服务
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            // 添加权限授权处理程序
            services.AddPermissionAuthorization();
            
            // 添加日志服务
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

            return services;
        }
    }
    
    /// <summary>
    /// Serilog日志适配器
    /// </summary>
    public class LoggerAdapter<T> : IAppLogger<T>
    {
        private readonly ILogger _logger;

        public LoggerAdapter()
        {
            _logger = Log.ForContext<T>();
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
    }
}