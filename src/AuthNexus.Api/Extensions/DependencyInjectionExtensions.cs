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
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero // 设置时钟偏差为零，使令牌在过期时间准确失效
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // 输出调试信息，帮助确定令牌是否正确传递
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        Console.WriteLine($"Received token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}...");
                        return Task.CompletedTask;
                    }
                };
            });
            
            return services;
        }
    }
}