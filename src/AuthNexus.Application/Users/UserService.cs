using AuthNexus.Application.Common;
using AuthNexus.Application.Permissions;
using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;

namespace AuthNexus.Application.Users;

/// <summary>
/// 用户管理服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public UserService(
        IUserRepository userRepository,
        IApplicationRepository applicationRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    public async Task<ResultDto<UserDto>> GetUserAsync(string appUid, string externalUserId)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<UserDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 查找用户，如果不存在则创建
            var user = await _userRepository.GetByExternalIdAsync(application.Id, externalUserId);
            if (user == null)
            {
                user = new UserIdentity(application.Id, externalUserId);
                user = await _userRepository.CreateAsync(user);
            }

            // 获取用户的完整信息（包括角色和直接权限）
            var userWithDetails = await _userRepository.GetWithRolesAndPermissionsAsync(user.Id);

            return ResultDto<UserDto>.Success(MapToDto(userWithDetails));
        }
        catch (Exception ex)
        {
            return ResultDto<UserDto>.Failure($"获取用户信息失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    public async Task<ResultDto<UserDto>> AssignRolesToUserAsync(string appUid, string externalUserId, AssignRolesToUserRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<UserDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 查找用户，如果不存在则创建
            var user = await _userRepository.GetByExternalIdAsync(application.Id, externalUserId);
            if (user == null)
            {
                user = new UserIdentity(application.Id, externalUserId);
                user = await _userRepository.CreateAsync(user);
            }

            // 获取用户的完整信息（包括角色）
            user = await _userRepository.GetWithRolesAsync(user.Id);

            // 确保所有角色名称都是有效的
            var roleList = new List<Role>();
            foreach (var name in request.RoleNames)
            {
                var role = await _roleRepository.GetByNameAsync(application.Id, name);
                if (role == null)
                {
                    return ResultDto<UserDto>.Failure($"角色 '{name}' 不存在于应用 '{appUid}' 中");
                }
                roleList.Add(role);
            }

            // 分配角色到用户
            foreach (var role in roleList)
            {
                user.AssignRole(role);
            }
            
            // 保存更新
            var updatedUser = await _userRepository.UpdateAsync(user);
            
            // 获取更新后的用户信息（包括角色和直接权限）
            var refreshedUser = await _userRepository.GetWithRolesAndPermissionsAsync(updatedUser.Id);

            return ResultDto<UserDto>.Success(MapToDto(refreshedUser));
        }
        catch (Exception ex)
        {
            return ResultDto<UserDto>.Failure($"为用户分配角色失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从用户移除角色
    /// </summary>
    public async Task<ResultDto<UserDto>> RemoveRolesFromUserAsync(string appUid, string externalUserId, RemoveRolesFromUserRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<UserDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取用户
            var user = await _userRepository.GetByExternalIdAsync(application.Id, externalUserId);
            if (user == null)
            {
                return ResultDto<UserDto>.Failure($"用户 '{externalUserId}' 不存在于应用 '{appUid}' 中");
            }

            // 获取用户的完整信息（包括角色）
            user = await _userRepository.GetWithRolesAsync(user.Id);

            // 获取要移除的角色ID列表
            var roleIds = new List<Guid>();
            foreach (var name in request.RoleNames)
            {
                var role = await _roleRepository.GetByNameAsync(application.Id, name);
                if (role != null)
                {
                    roleIds.Add(role.Id);
                }
            }

            // 从用户移除角色
            foreach (var roleId in roleIds)
            {
                user.RemoveRole(roleId);
            }
            
            // 保存更新
            var updatedUser = await _userRepository.UpdateAsync(user);
            
            // 获取更新后的用户信息（包括角色和直接权限）
            var refreshedUser = await _userRepository.GetWithRolesAndPermissionsAsync(updatedUser.Id);

            return ResultDto<UserDto>.Success(MapToDto(refreshedUser));
        }
        catch (Exception ex)
        {
            return ResultDto<UserDto>.Failure($"从用户移除角色失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 为用户分配直接权限
    /// </summary>
    public async Task<ResultDto<UserDto>> AssignDirectPermissionsToUserAsync(string appUid, string externalUserId, AssignDirectPermissionsToUserRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<UserDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 查找用户，如果不存在则创建
            var user = await _userRepository.GetByExternalIdAsync(application.Id, externalUserId);
            if (user == null)
            {
                user = new UserIdentity(application.Id, externalUserId);
                user = await _userRepository.CreateAsync(user);
            }

            // 获取用户的完整信息（包括直接权限）
            user = await _userRepository.GetWithDirectPermissionsAsync(user.Id);

            // 确保所有权限代码都是有效的
            var permissionList = new List<PermissionDefinition>();
            foreach (var code in request.PermissionCodes)
            {
                var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
                if (permission == null)
                {
                    return ResultDto<UserDto>.Failure($"权限代码 '{code}' 不存在于应用 '{appUid}' 中");
                }
                permissionList.Add(permission);
            }

            // 分配直接权限到用户
            foreach (var permission in permissionList)
            {
                user.AssignDirectPermission(permission);
            }
            
            // 保存更新
            var updatedUser = await _userRepository.UpdateAsync(user);
            
            // 获取更新后的用户信息（包括角色和直接权限）
            var refreshedUser = await _userRepository.GetWithRolesAndPermissionsAsync(updatedUser.Id);

            return ResultDto<UserDto>.Success(MapToDto(refreshedUser));
        }
        catch (Exception ex)
        {
            return ResultDto<UserDto>.Failure($"为用户分配直接权限失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从用户移除直接权限
    /// </summary>
    public async Task<ResultDto<UserDto>> RemoveDirectPermissionsFromUserAsync(string appUid, string externalUserId, RemoveDirectPermissionsFromUserRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<UserDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取用户
            var user = await _userRepository.GetByExternalIdAsync(application.Id, externalUserId);
            if (user == null)
            {
                return ResultDto<UserDto>.Failure($"用户 '{externalUserId}' 不存在于应用 '{appUid}' 中");
            }

            // 获取用户的完整信息（包括直接权限）
            user = await _userRepository.GetWithDirectPermissionsAsync(user.Id);

            // 获取要移除的权限ID列表
            var permissionIds = new List<Guid>();
            foreach (var code in request.PermissionCodes)
            {
                var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
                if (permission != null)
                {
                    permissionIds.Add(permission.Id);
                }
            }

            // 从用户移除直接权限
            foreach (var permissionId in permissionIds)
            {
                user.RemoveDirectPermission(permissionId);
            }
            
            // 保存更新
            var updatedUser = await _userRepository.UpdateAsync(user);
            
            // 获取更新后的用户信息（包括角色和直接权限）
            var refreshedUser = await _userRepository.GetWithRolesAndPermissionsAsync(updatedUser.Id);

            return ResultDto<UserDto>.Success(MapToDto(refreshedUser));
        }
        catch (Exception ex)
        {
            return ResultDto<UserDto>.Failure($"从用户移除直接权限失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取用户的所有权限
    /// </summary>
    public async Task<ResultDto<IEnumerable<PermissionDefinitionDto>>> GetUserPermissionsAsync(string appUid, string externalUserId)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<IEnumerable<PermissionDefinitionDto>>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取用户的所有权限
            var permissions = await _userRepository.GetAllPermissionsAsync(application.Id, externalUserId);
            
            // 转换为DTO
            var permissionDtos = permissions.Select(p => new PermissionDefinitionDto
            {
                Id = p.Id,
                ApplicationId = p.ApplicationId,
                Code = p.Code,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });

            return ResultDto<IEnumerable<PermissionDefinitionDto>>.Success(permissionDtos);
        }
        catch (Exception ex)
        {
            return ResultDto<IEnumerable<PermissionDefinitionDto>>.Failure($"获取用户权限列表失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 校验用户权限
    /// </summary>
    public async Task<ResultDto<bool>> CheckPermissionAsync(CheckPermissionRequest request)
    {
        return await CheckPermissionAsync(request.AppUID, request.ExternalUserId, request.PermissionCode);
    }

    /// <summary>
    /// 校验用户权限
    /// </summary>
    public async Task<ResultDto<bool>> CheckPermissionAsync(string appUid, string externalUserId, string permissionCode)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<bool>.Failure($"应用 '{appUid}' 不存在");
            }

            // 如果应用已禁用，直接返回无权限
            if (!application.IsEnabled)
            {
                return ResultDto<bool>.Success(false);
            }

            // 检查用户是否拥有指定权限
            var hasPermission = await _userRepository.HasPermissionAsync(application.Id, externalUserId, permissionCode);

            return ResultDto<bool>.Success(hasPermission);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure($"校验用户权限失败: {ex.Message}");
        }
    }

    // 辅助方法

    /// <summary>
    /// 将用户实体映射为DTO
    /// </summary>
    private UserDto MapToDto(UserIdentity user)
    {
        return new UserDto
        {
            Id = user.Id,
            ApplicationId = user.ApplicationId,
            ExternalUserId = user.ExternalUserId,
            CreatedAt = user.CreatedAt,
            RoleIds = user.Roles.Select(r => r.RoleId),
            DirectPermissionIds = user.DirectPermissions.Select(p => p.PermissionDefinitionId)
        };
    }
}