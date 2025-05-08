using MediatR;
using AuthNexus.SharedKernel.Interfaces;
using AuthNexus.SharedKernel.Exceptions;
using System.Reflection;

namespace AuthNexus.Application.Behaviors
{
    /// <summary>
    /// 授权行为 - 用于检查请求处理前的权限
    /// </summary>
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 获取请求类型上的授权特性
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
            
            // 如果没有授权特性，则直接通过
            if (!authorizeAttributes.Any())
            {
                return await next();
            }
            
            // 如果用户未登录，则抛出未授权异常
            if (_currentUserService.UserId == null)
            {
                throw new UnauthorizedException();
            }
            
            // 检查每个授权特性
            foreach (var attribute in authorizeAttributes)
            {
                // 检查角色
                if (!string.IsNullOrWhiteSpace(attribute.Roles))
                {
                    var roles = attribute.Roles.Split(',').Select(r => r.Trim());
                    var userRoles = _currentUserService.GetRoles();
                    
                    if (!roles.Any(r => userRoles.Contains(r)))
                    {
                        throw new ForbiddenException();
                    }
                }
                
                // 检查权限
                if (!string.IsNullOrWhiteSpace(attribute.Permissions))
                {
                    var permissions = attribute.Permissions.Split(',').Select(p => p.Trim());
                    
                    foreach (var permission in permissions)
                    {
                        if (!_currentUserService.HasPermission(permission))
                        {
                            throw new ForbiddenException($"缺少所需权限: {permission}");
                        }
                    }
                }
            }
            
            // 通过授权检查，继续处理请求
            return await next();
        }
    }
    
    /// <summary>
    /// 授权特性 - 用于标记需要授权的请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>
        /// 逗号分隔的角色列表
        /// </summary>
        public string Roles { get; set; } = string.Empty;
        
        /// <summary>
        /// 逗号分隔的权限列表
        /// </summary>
        public string Permissions { get; set; } = string.Empty;
    }
}