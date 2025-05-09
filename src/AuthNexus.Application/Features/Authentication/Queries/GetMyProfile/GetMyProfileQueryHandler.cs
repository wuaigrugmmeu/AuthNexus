using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.Domain.Repositories;
using AuthNexus.SharedKernel.Exceptions;
using AuthNexus.SharedKernel.Interfaces;
using AuthNexus.SharedKernel.Models;
using AutoMapper;
using MediatR;

namespace AuthNexus.Application.Features.Authentication.Queries.GetMyProfile
{
    /// <summary>
    /// 获取当前用户个人资料查询处理程序
    /// </summary>
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetMyProfileQueryHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result<UserProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 获取当前用户ID
                var userId = _currentUserService.UserId;
                Console.WriteLine($"Current user ID from service: {userId}");
                
                if (userId == null)
                {
                    Console.WriteLine("CurrentUserService.UserId returned null - user is not authenticated");
                    return Result.Failure<UserProfileDto>("用户未登录");
                }

                // 获取当前用户名
                var username = _currentUserService.Username;
                Console.WriteLine($"Current username from service: {username}");
                
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("CurrentUserService.Username returned null or empty - user info is incomplete");
                    return Result.Failure<UserProfileDto>("用户信息不完整");
                }

                // 使用和登录时相同的方法获取用户信息
                var user = await _userRepository.GetByUsernameOrEmailAsync(username);
                
                if (user == null)
                {
                    Console.WriteLine($"User with username {username} not found in database");
                    return Result.Failure<UserProfileDto>($"未找到用户: {username}");
                }
                
                Console.WriteLine($"Found user: {user.Id} in database");

                // 获取用户角色和权限
                var roles = await _userRepository.GetUserRolesAsync(user.Id);
                var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);
                
                Console.WriteLine($"User has {roles.Count()} roles and {permissions.Count()} permissions");

                // 映射到DTO
                var userProfileDto = new UserProfileDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = roles.Select(r => r.Name).ToList(),
                    Permissions = permissions.Select(p => p.Name).ToList()
                };

                return Result.Success(userProfileDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetMyProfileQueryHandler: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Result.Failure<UserProfileDto>($"获取用户信息失败: {ex.Message}");
            }
        }
    }
}