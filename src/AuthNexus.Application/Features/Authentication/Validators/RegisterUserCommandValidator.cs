using FluentValidation;
using AuthNexus.Application.Features.Authentication.Commands.RegisterUser;

namespace AuthNexus.Application.Features.Authentication.Validators
{
    /// <summary>
    /// 用户注册命令验证器
    /// </summary>
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("用户名不能为空")
                .MinimumLength(3).WithMessage("用户名长度不能少于3个字符")
                .MaximumLength(50).WithMessage("用户名长度不能超过50个字符")
                .Matches("^[a-zA-Z0-9_-]+$").WithMessage("用户名只能包含字母、数字、下划线和连字符");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("邮箱不能为空")
                .EmailAddress().WithMessage("邮箱格式不正确")
                .MaximumLength(100).WithMessage("邮箱长度不能超过100个字符");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空")
                .MinimumLength(6).WithMessage("密码长度不能少于6个字符")
                .MaximumLength(100).WithMessage("密码长度不能超过100个字符")
                .Matches("[A-Z]").WithMessage("密码必须包含至少一个大写字母")
                .Matches("[a-z]").WithMessage("密码必须包含至少一个小写字母")
                .Matches("[0-9]").WithMessage("密码必须包含至少一个数字")
                .Matches("[^a-zA-Z0-9]").WithMessage("密码必须包含至少一个特殊字符");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("确认密码不能为空")
                .Equal(x => x.Password).WithMessage("两次输入的密码不一致");
        }
    }
}