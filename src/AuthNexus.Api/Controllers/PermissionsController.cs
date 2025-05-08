using AuthNexus.Application.Common;
using AuthNexus.Application.Permissions;
using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/[controller]")]
[Authorize] // 需要用户登录
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// 获取所有权限
    /// </summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看权限的权限
    public async Task<ActionResult<IEnumerable<PermissionDefinitionDto>>> GetAllPermissions(Guid applicationId)
    {
        var result = await _permissionService.GetApplicationPermissionsAsync(applicationId.ToString());
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// 根据代码获取权限
    /// </summary>
    [HttpGet("{permissionCode}")]
    [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看权限的权限
    public async Task<ActionResult<PermissionDefinitionDto>> GetPermission(Guid applicationId, string permissionCode)
    {
        var result = await _permissionService.GetPermissionAsync(applicationId.ToString(), permissionCode);
        if (!result.IsSuccess)
        {
            return NotFound(ResultDto.Failure("权限不存在"));
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// 创建新权限
    /// </summary>
    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult<PermissionDefinitionDto>> CreatePermission(Guid applicationId, CreatePermissionRequest request)
    {
        // 确保应用ID匹配
        if (applicationId.ToString() != request.ApplicationId)
        {
            return BadRequest(ResultDto.Failure("应用ID不匹配"));
        }

        var result = await _permissionService.CreatePermissionAsync(applicationId.ToString(), request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetPermission), 
            new { applicationId = applicationId, permissionCode = result.Data.Code }, 
            result.Data);
    }

    /// <summary>
    /// 更新权限
    /// </summary>
    [HttpPut("{permissionCode}")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult<PermissionDefinitionDto>> UpdatePermission(Guid applicationId, string permissionCode, UpdatePermissionRequest request)
    {
        if (permissionCode != request.Code || applicationId.ToString() != request.ApplicationId)
        {
            return BadRequest(ResultDto.Failure("ID不匹配"));
        }

        var result = await _permissionService.UpdatePermissionAsync(applicationId.ToString(), permissionCode, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// 删除权限
    /// </summary>
    [HttpDelete("{permissionCode}")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult> DeletePermission(Guid applicationId, string permissionCode)
    {
        var result = await _permissionService.DeletePermissionAsync(applicationId.ToString(), permissionCode);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }
}