using AuthNexus.Application.Applications;
using AuthNexus.Application.Permissions;
using AuthNexus.Application.Roles;
using AuthNexus.Application.Users;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AuthNexus.Application.Behaviors;
using FluentValidation;
using MediatR;

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

        // 添加MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            // 添加管道行为
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        });
        
        // 添加FluentValidation - 修改为使用Assembly注册方式，避免依赖FluentValidation.DependencyInjection
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        foreach (var assembly in new[] { Assembly.GetExecutingAssembly() })
        {
            foreach (var validatorType in assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IValidator<>))))
            {
                var validatorInterface = validatorType.GetInterfaces()
                    .First(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == typeof(IValidator<>));
                    
                services.AddTransient(validatorInterface, validatorType);
            }
        }
        
        // 添加AutoMapper - 使用特定版本的方法签名避免歧义
        services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);
        
        return services;
    }
}