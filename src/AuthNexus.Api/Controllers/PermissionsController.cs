using AuthNexus.Application.Common;
using AuthNexus.Application.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDefinitionDto>>> GetAllPermissions(Guid applicationId)
    {
        var result = await _permissionService.GetApplicationPermissionsAsync(applicationId.ToString());
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<PermissionDefinitionDto>> GetPermission(Guid applicationId, string code)
    {
        var result = await _permissionService.GetPermissionAsync(applicationId.ToString(), code);
        if (!result.IsSuccess)
        {
            return NotFound(ResultDto.Failure("权限不存在"));
        }
        return Ok(result.Data);
    }

    [HttpPost]
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
            new { applicationId = applicationId, code = result.Data.Code }, 
            result.Data);
    }

    [HttpPut("{code}")]
    public async Task<ActionResult<PermissionDefinitionDto>> UpdatePermission(Guid applicationId, string code, UpdatePermissionRequest request)
    {
        if (code != request.Code || applicationId.ToString() != request.ApplicationId)
        {
            return BadRequest(ResultDto.Failure("ID不匹配"));
        }

        var result = await _permissionService.UpdatePermissionAsync(applicationId.ToString(), code, request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{code}")]
    public async Task<ActionResult> DeletePermission(Guid applicationId, string code)
    {
        var result = await _permissionService.DeletePermissionAsync(applicationId.ToString(), code);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return NoContent();
    }
}