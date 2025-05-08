using AuthNexus.Api.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthNexus.Api.Extensions
{
    /// <summary>
    /// 依赖注入扩展方法
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 添加自定义中间件
        /// </summary>
        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            // 添加关联ID中间件
            app.UseMiddleware<CorrelationIdMiddleware>();
            
            // 添加请求日志中间件
            app.UseMiddleware<RequestLoggingMiddleware>();
            
            // 添加全局异常处理中间件
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            
            return app;
        }
        
        /// <summary>
        /// 配置JWT认证
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };
            });
            
            return services;
        }
    }
}