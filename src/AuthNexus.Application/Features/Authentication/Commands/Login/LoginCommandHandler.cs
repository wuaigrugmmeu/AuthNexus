using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.Domain.Repositories;
using AuthNexus.Application.Common;
using MediatR;
using AuthNexus.Domain.Services;

namespace AuthNexus.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// 登录命令处理程序
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResultDto<TokenResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ResultDto<TokenResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 查找用户
            var user = await _userRepository.GetByUsernameOrEmailAsync(request.Username);
            if (user == null)
            {
                return ResultDto<TokenResponseDto>.Failure("用户名或密码不正确");
            }

            // 验证密码
            if (!_passwordHashingService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return ResultDto<TokenResponseDto>.Failure("用户名或密码不正确");
            }

            // 获取用户角色和权限
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            // 生成令牌
            var tokenResponse = _jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Username,
                roles.Select(r => r.Name).ToList(),
                permissions.Select(p => p.Name).ToList());

            // 将Domain层的TokenResponse映射为Application层的TokenResponseDto
            var tokenResponseDto = new TokenResponseDto
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                UserId = user.Id  // 设置用户ID
            };

            return ResultDto<TokenResponseDto>.Success(tokenResponseDto);
        }
    }
}