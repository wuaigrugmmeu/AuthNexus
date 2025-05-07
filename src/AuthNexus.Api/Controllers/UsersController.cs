using AuthNexus.Application.Common;
using AuthNexus.Application.Users;
using AuthNexus.Application.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{externalUserId}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid applicationId, string externalUserId)
    {
        var result = await _userService.GetUserAsync(applicationId.ToString(), externalUserId);
        if (!result.IsSuccess)
        {
            return NotFound(ResultDto.Failure("用户不存在"));
        }
        return Ok(result.Data);
    }

    [HttpPost("{externalUserId}/roles")]
    public async Task<ActionResult> AssignRolesToUser(Guid applicationId, string externalUserId, AssignRolesToUserRequest request)
    {
        // 确保应用ID和用户ID匹配
        if (applicationId != request.ApplicationId || externalUserId != request.ExternalUserId)
        {
            return BadRequest(ResultDto.Failure("ID不匹配"));
        }

        var result = await _userService.AssignRolesToUserAsync(applicationId.ToString(), externalUserId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{externalUserId}/roles/{roleId}")]
    public async Task<ActionResult> RemoveRoleFromUser(Guid applicationId, string externalUserId, Guid roleId)
    {
        var request = new RemoveRolesFromUserRequest
        {
            RoleNames = new[] { roleId.ToString() }
        };
        
        var result = await _userService.RemoveRolesFromUserAsync(applicationId.ToString(), externalUserId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }

    [HttpPost("{externalUserId}/permissions")]
    public async Task<ActionResult> AssignDirectPermissionsToUser(Guid applicationId, string externalUserId, AssignDirectPermissionsToUserRequest request)
    {
        // 确保应用ID和用户ID匹配
        if (applicationId != request.ApplicationId || externalUserId != request.ExternalUserId)
        {
            return BadRequest(ResultDto.Failure("ID不匹配"));
        }

        var result = await _userService.AssignDirectPermissionsToUserAsync(applicationId.ToString(), externalUserId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{externalUserId}/permissions/{permissionId}")]
    public async Task<ActionResult> RemoveDirectPermissionFromUser(Guid applicationId, string externalUserId, Guid permissionId)
    {
        var request = new RemoveDirectPermissionsFromUserRequest
        {
            PermissionCodes = new[] { permissionId.ToString() }
        };
        
        var result = await _userService.RemoveDirectPermissionsFromUserAsync(applicationId.ToString(), externalUserId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }

    [HttpGet("{externalUserId}/permissions")]
    public async Task<ActionResult<IEnumerable<PermissionDefinitionDto>>> GetUserPermissions(Guid applicationId, string externalUserId)
    {
        var result = await _userService.GetUserPermissionsAsync(applicationId.ToString(), externalUserId);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        
        return Ok(result.Data);
    }

    [HttpPost("check-permission")]
    public async Task<ActionResult<bool>> CheckPermission(Guid applicationId, CheckPermissionRequest request)
    {
        var result = await _userService.CheckPermissionAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(ResultDto.Failure("权限验证失败"));
        }

        return Ok(result.Data);
    }
}