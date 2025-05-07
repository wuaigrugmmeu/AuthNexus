using AuthNexus.Application.Applications;
using AuthNexus.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApplicationRegistrationResultDto>> RegisterApplication(RegisterApplicationRequest request)
    {
        var result = await _applicationService.RegisterApplicationAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAllApplications()
    {
        var result = await _applicationService.GetAllApplicationsAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetApplication(Guid id)
    {
        var result = await _applicationService.GetApplicationAsync(id.ToString());
        if (!result.IsSuccess)
        {
            return NotFound(ResultDto.Failure("应用不存在"));
        }
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationDto>> UpdateApplication(Guid id, UpdateApplicationRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest(ResultDto.Failure("ID不匹配"));
        }

        var result = await _applicationService.UpdateApplicationAsync(id.ToString(), request);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpPost("{id}/regenerate-keys")]
    public async Task<ActionResult<ApplicationRegistrationResultDto>> RegenerateKeys(Guid id)
    {
        var result = await _applicationService.RegenerateKeysAsync(id);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpPost("{id}/enable")]
    public async Task<ActionResult> EnableApplication(Guid id)
    {
        var result = await _applicationService.EnableApplicationAsync(id.ToString());
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(ResultDto.Success());
    }

    [HttpPost("{id}/disable")]
    public async Task<ActionResult> DisableApplication(Guid id)
    {
        var result = await _applicationService.DisableApplicationAsync(id.ToString());
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(ResultDto.Success());
    }
}