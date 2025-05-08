using FluentValidation;
using AuthNexus.Application.Features.Authentication.Commands.Login;

namespace AuthNexus.Application.Features.Authentication.Validators
{
    /// <summary>
    /// 登录命令验证器
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("用户名不能为空")
                .MaximumLength(100).WithMessage("用户名最大长度为100个字符");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空")
                .MinimumLength(6).WithMessage("密码长度不能少于6个字符");
        }
    }
}