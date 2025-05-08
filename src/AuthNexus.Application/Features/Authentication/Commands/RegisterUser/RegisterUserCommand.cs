using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.SharedKernel.Models;
using MediatR;

namespace AuthNexus.Application.Features.Authentication.Commands.RegisterUser
{
    /// <summary>
    /// 用户注册命令
    /// </summary>
    public class RegisterUserCommand : IRequest<Result<UserProfileDto>>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}