using AuthNexus.Application.Common;
using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;

namespace AuthNexus.Application.Roles;

/// <summary>
/// 角色管理服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(
        IRoleRepository roleRepository,
        IApplicationRepository applicationRepository,
        IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public async Task<ResultDto<RoleDto>> CreateRoleAsync(string appUid, CreateRoleRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<RoleDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 检查角色名称是否已存在
            if (await _roleRepository.ExistsByNameAsync(application.Id, request.Name))
            {
                return ResultDto<RoleDto>.Failure($"角色名称 '{request.Name}' 已存在于应用 '{appUid}' 中");
            }

            // 创建角色
            var role = new Role(
                application.Id,
                request.Name,
                request.Description
            );

            // 保存角色
            var createdRole = await _roleRepository.CreateAsync(role);

            return ResultDto<RoleDto>.Success(MapToDto(createdRole));
        }
        catch (Exception ex)
        {
            return ResultDto<RoleDto>.Failure($"创建角色失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取角色信息
    /// </summary>
    public async Task<ResultDto<RoleDto>> GetRoleAsync(string appUid, string roleName)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<RoleDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取角色
            var role = await _roleRepository.GetWithPermissionsAsync(
                (await _roleRepository.GetByNameAsync(application.Id, roleName))?.Id ?? Guid.Empty
            );
            
            if (role == null)
            {
                return ResultDto<RoleDto>.Failure($"角色 '{roleName}' 不存在于应用 '{appUid}' 中");
            }

            return ResultDto<RoleDto>.Success(MapToDto(role));
        }
        catch (Exception ex)
        {
            return ResultDto<RoleDto>.Failure($"获取角色信息失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task<ResultDto<RoleDto>> UpdateRoleAsync(string appUid, string roleName, UpdateRoleRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<RoleDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取角色
            var role = await _roleRepository.GetByNameAsync(application.Id, roleName);
            if (role == null)
            {
                return ResultDto<RoleDto>.Failure($"角色 '{roleName}' 不存在于应用 '{appUid}' 中");
            }

            // 如果新名称与当前名称不同，检查新名称是否已存在
            if (request.Name != roleName && await _roleRepository.ExistsByNameAsync(application.Id, request.Name))
            {
                return ResultDto<RoleDto>.Failure($"角色名称 '{request.Name}' 已存在于应用 '{appUid}' 中");
            }

            // 更新角色信息
            role.Update(request.Name, request.Description);
            
            // 保存更新
            var updatedRole = await _roleRepository.UpdateAsync(role);

            // 获取包含权限的完整角色信息
            var roleWithPermissions = await _roleRepository.GetWithPermissionsAsync(updatedRole.Id);

            return ResultDto<RoleDto>.Success(MapToDto(roleWithPermissions));
        }
        catch (Exception ex)
        {
            return ResultDto<RoleDto>.Failure($"更新角色失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task<ResultDto> DeleteRoleAsync(string appUid, string roleName)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取角色
            var role = await _roleRepository.GetByNameAsync(application.Id, roleName);
            if (role == null)
            {
                return ResultDto.Failure($"角色 '{roleName}' 不存在于应用 '{appUid}' 中");
            }

            // 删除角色
            await _roleRepository.DeleteAsync(role.Id);

            return ResultDto.Success();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"删除角色失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取应用的所有角色
    /// </summary>
    public async Task<ResultDto<IEnumerable<RoleDto>>> GetApplicationRolesAsync(string appUid)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<IEnumerable<RoleDto>>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取应用的所有角色
            var roles = await _roleRepository.GetByApplicationIdAsync(application.Id);
            
            // 对每个角色加载权限信息
            var rolesWithPermissions = new List<RoleDto>();
            foreach (var role in roles)
            {
                var roleWithPermissions = await _roleRepository.GetWithPermissionsAsync(role.Id);
                rolesWithPermissions.Add(MapToDto(roleWithPermissions));
            }

            return ResultDto<IEnumerable<RoleDto>>.Success(rolesWithPermissions);
        }
        catch (Exception ex)
        {
            return ResultDto<IEnumerable<RoleDto>>.Failure($"获取应用角色列表失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    public async Task<ResultDto<RoleDto>> AssignPermissionsToRoleAsync(string appUid, string roleName, AssignPermissionsToRoleRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<RoleDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取角色
            var role = await _roleRepository.GetWithPermissionsAsync(
                (await _roleRepository.GetByNameAsync(application.Id, roleName))?.Id ?? Guid.Empty
            );
            
            if (role == null)
            {
                return ResultDto<RoleDto>.Failure($"角色 '{roleName}' 不存在于应用 '{appUid}' 中");
            }

            // 确保所有权限代码都是有效的
            var permissionList = new List<PermissionDefinition>();
            foreach (var code in request.PermissionCodes)
            {
                var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
                if (permission == null)
                {
                    return ResultDto<RoleDto>.Failure($"权限代码 '{code}' 不存在于应用 '{appUid}' 中");
                }
                permissionList.Add(permission);
            }

            // 分配权限到角色
            foreach (var permission in permissionList)
            {
                role.AddPermission(permission);
            }
            
            // 保存更新
            var updatedRole = await _roleRepository.UpdateAsync(role);
            
            // 获取更新后的角色信息（包含权限）
            var refreshedRole = await _roleRepository.GetWithPermissionsAsync(updatedRole.Id);

            return ResultDto<RoleDto>.Success(MapToDto(refreshedRole));
        }
        catch (Exception ex)
        {
            return ResultDto<RoleDto>.Failure($"为角色分配权限失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从角色移除权限
    /// </summary>
    public async Task<ResultDto<RoleDto>> RemovePermissionsFromRoleAsync(string appUid, string roleName, RemovePermissionsFromRoleRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<RoleDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取角色
            var role = await _roleRepository.GetWithPermissionsAsync(
                (await _roleRepository.GetByNameAsync(application.Id, roleName))?.Id ?? Guid.Empty
            );
            
            if (role == null)
            {
                return ResultDto<RoleDto>.Failure($"角色 '{roleName}' 不存在于应用 '{appUid}' 中");
            }

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

            // 从角色移除权限
            foreach (var permissionId in permissionIds)
            {
                role.RemovePermission(permissionId);
            }
            
            // 保存更新
            var updatedRole = await _roleRepository.UpdateAsync(role);
            
            // 获取更新后的角色信息（包含权限）
            var refreshedRole = await _roleRepository.GetWithPermissionsAsync(updatedRole.Id);

            return ResultDto<RoleDto>.Success(MapToDto(refreshedRole));
        }
        catch (Exception ex)
        {
            return ResultDto<RoleDto>.Failure($"从角色移除权限失败: {ex.Message}");
        }
    }

    // 辅助方法

    /// <summary>
    /// 将角色实体映射为DTO
    /// </summary>
    private RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            ApplicationId = role.ApplicationId,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            PermissionIds = role.Permissions.Select(p => p.PermissionDefinitionId)
        };
    }
}