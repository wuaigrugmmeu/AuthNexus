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
            // 获取当前用户ID
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new UnauthorizedException("用户未登录");
            }

            // 获取用户信息
            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                throw new EntityNotFoundException("User", userId.Value);
            }

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