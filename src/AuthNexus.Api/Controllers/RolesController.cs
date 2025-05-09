using AuthNexus.Application.Common;
using AuthNexus.Application.Roles;
using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/[controller]")]
[Authorize] // 需要用户登录
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看角色权限
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles(Guid applicationId)
    {
        var result = await _roleService.GetApplicationRolesAsync(applicationId.ToString());
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    [HttpGet("{roleName}")]
    [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看角色权限
    public async Task<ActionResult<RoleDto>> GetRole(Guid applicationId, string roleName)
    {
        var result = await _roleService.GetRoleAsync(applicationId.ToString(), roleName);
        if (!result.IsSuccess)
        {
            return NotFound(ResultDto.Failure("角色不存在"));
        }
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult<RoleDto>> CreateRole(Guid applicationId, CreateRoleRequest request)
    {
        // 确保请求中的应用ID与路由中的应用ID匹配
        var result = await _roleService.CreateRoleAsync(applicationId.ToString(), request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetRole), 
            new { applicationId = applicationId, roleName = result.Data.Name }, 
            result.Data);
    }

    [HttpPut("{roleName}")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult<RoleDto>> UpdateRole(Guid applicationId, string roleName, UpdateRoleRequest request)
    {
        var result = await _roleService.UpdateRoleAsync(applicationId.ToString(), roleName, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{roleName}")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult> DeleteRole(Guid applicationId, string roleName)
    {
        var result = await _roleService.DeleteRoleAsync(applicationId.ToString(), roleName);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }

    [HttpPost("{roleName}/permissions")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult> AssignPermissionsToRole(Guid applicationId, string roleName, AssignPermissionsToRoleRequest request)
    {
        var result = await _roleService.AssignPermissionsToRoleAsync(applicationId.ToString(), roleName, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{roleName}/permissions")]
    [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
    public async Task<ActionResult> RemovePermissionsFromRole(Guid applicationId, string roleName, RemovePermissionsFromRoleRequest request)
    {
        var result = await _roleService.RemovePermissionsFromRoleAsync(applicationId.ToString(), roleName, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }
}