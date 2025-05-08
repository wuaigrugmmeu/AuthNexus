using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;
using AuthNexus.Domain.Services;
using AuthNexus.SharedKernel.Models;
using AutoMapper;
using MediatR;

namespace AuthNexus.Application.Features.Authentication.Commands.RegisterUser
{
    /// <summary>
    /// 用户注册命令处理程序
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHashingService passwordHashingService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHashingService = passwordHashingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserProfileDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 检查用户名是否已存在
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                return Result.Failure<UserProfileDto>($"用户名 '{request.Username}' 已被使用", 400);
            }

            // 检查邮箱是否已存在
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                return Result.Failure<UserProfileDto>($"邮箱 '{request.Email}' 已被使用", 400);
            }

            // 创建新用户
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHashingService.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // 为用户分配默认角色
            var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole != null)
            {
                await _userRepository.AssignRoleToUserAsync(user.Id, defaultRole.Id);
            }

            // 提交事务
            await _unitOfWork.SaveChangesAsync();

            // 获取用户角色和权限
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            // 映射到DTO
            var userProfileDto = _mapper.Map<UserProfileDto>(user);
            userProfileDto.Roles = roles.Select(r => r.Name).ToList();
            userProfileDto.Permissions = permissions.Select(p => p.Name).ToList();

            return Result.Success(userProfileDto);
        }
    }
}