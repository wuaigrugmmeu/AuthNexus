using AuthNexus.Api.Middlewares;
using Microsoft.OpenApi.Models;

namespace AuthNexus.Api.Extensions
{
    /// <summary>
    /// Swagger配置扩展方法
    /// </summary>
    public static class SwaggerConfigurationExtensions
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AuthNexus API",
                    Version = "v1",
                    Description = "权限管理系统API",
                    Contact = new OpenApiContact
                    {
                        Name = "AuthNexus Team"
                    }
                });
                
                // 添加JWT认证设置
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            
            return services;
        }
    }
}