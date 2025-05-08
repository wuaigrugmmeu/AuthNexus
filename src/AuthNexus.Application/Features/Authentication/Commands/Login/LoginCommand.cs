using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.SharedKernel.Models;
using MediatR;

namespace AuthNexus.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// 登录命令
    /// </summary>
    public class LoginCommand : IRequest<Result<TokenResponseDto>>
    {
        /// <summary>
        /// 用户名/邮箱
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}