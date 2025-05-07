using AuthNexus.Application.Common;
using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;

namespace AuthNexus.Application.Permissions;

/// <summary>
/// 权限管理服务实现
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IApplicationRepository _applicationRepository;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IApplicationRepository applicationRepository)
    {
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
    }

    /// <summary>
    /// 创建权限定义
    /// </summary>
    public async Task<ResultDto<PermissionDefinitionDto>> CreatePermissionAsync(string appUid, CreatePermissionRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 检查权限代码是否已存在
            if (await _permissionRepository.ExistsByCodeAsync(application.Id, request.Code))
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"权限代码 '{request.Code}' 已存在于应用 '{appUid}' 中");
            }

            // 创建权限定义
            var permission = new PermissionDefinition(
                application.Id,
                request.Code,
                request.Description
            );

            // 保存权限定义
            var createdPermission = await _permissionRepository.CreateAsync(permission);

            return ResultDto<PermissionDefinitionDto>.Success(MapToDto(createdPermission));
        }
        catch (Exception ex)
        {
            return ResultDto<PermissionDefinitionDto>.Failure($"创建权限定义失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取权限定义信息
    /// </summary>
    public async Task<ResultDto<PermissionDefinitionDto>> GetPermissionAsync(string appUid, string code)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取权限定义
            var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
            if (permission == null)
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"权限代码 '{code}' 不存在于应用 '{appUid}' 中");
            }

            return ResultDto<PermissionDefinitionDto>.Success(MapToDto(permission));
        }
        catch (Exception ex)
        {
            return ResultDto<PermissionDefinitionDto>.Failure($"获取权限定义失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新权限定义
    /// </summary>
    public async Task<ResultDto<PermissionDefinitionDto>> UpdatePermissionAsync(string appUid, string code, UpdatePermissionRequest request)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取权限定义
            var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
            if (permission == null)
            {
                return ResultDto<PermissionDefinitionDto>.Failure($"权限代码 '{code}' 不存在于应用 '{appUid}' 中");
            }

            // 更新权限定义
            permission.Update(request.Description);
            
            // 保存更新
            var updatedPermission = await _permissionRepository.UpdateAsync(permission);

            return ResultDto<PermissionDefinitionDto>.Success(MapToDto(updatedPermission));
        }
        catch (Exception ex)
        {
            return ResultDto<PermissionDefinitionDto>.Failure($"更新权限定义失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除权限定义
    /// </summary>
    public async Task<ResultDto> DeletePermissionAsync(string appUid, string code)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取权限定义
            var permission = await _permissionRepository.GetByCodeAsync(application.Id, code);
            if (permission == null)
            {
                return ResultDto.Failure($"权限代码 '{code}' 不存在于应用 '{appUid}' 中");
            }

            // 删除权限定义
            await _permissionRepository.DeleteAsync(permission.Id);

            return ResultDto.Success();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"删除权限定义失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取应用的所有权限定义
    /// </summary>
    public async Task<ResultDto<IEnumerable<PermissionDefinitionDto>>> GetApplicationPermissionsAsync(string appUid)
    {
        try
        {
            // 获取应用
            var application = await _applicationRepository.GetByAppUidAsync(appUid);
            if (application == null)
            {
                return ResultDto<IEnumerable<PermissionDefinitionDto>>.Failure($"应用 '{appUid}' 不存在");
            }

            // 获取应用的所有权限定义
            var permissions = await _permissionRepository.GetByApplicationIdAsync(application.Id);
            var permissionDtos = permissions.Select(MapToDto);

            return ResultDto<IEnumerable<PermissionDefinitionDto>>.Success(permissionDtos);
        }
        catch (Exception ex)
        {
            return ResultDto<IEnumerable<PermissionDefinitionDto>>.Failure($"获取应用权限列表失败: {ex.Message}");
        }
    }

    // 辅助方法

    /// <summary>
    /// 将权限定义实体映射为DTO
    /// </summary>
    private PermissionDefinitionDto MapToDto(PermissionDefinition permission)
    {
        return new PermissionDefinitionDto
        {
            Id = permission.Id,
            ApplicationId = permission.ApplicationId,
            Code = permission.Code,
            Description = permission.Description,
            CreatedAt = permission.CreatedAt,
            UpdatedAt = permission.UpdatedAt
        };
    }
}