using AuthNexus.Application.Applications;
using AuthNexus.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 需要用户登录
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// 获取所有应用程序
        /// </summary>
        [HttpGet]
        [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看应用的权限
        public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAllApplications()
        {
            var result = await _applicationService.GetAllApplicationsAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// 根据ID获取应用程序
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = PolicyNames.RequirePermission)] // 需要查看应用的权限
        public async Task<ActionResult<ApplicationDto>> GetApplicationById(Guid id)
        {
            var result = await _applicationService.GetApplicationByIdAsync(id.ToString());
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// 注册新应用程序
        /// </summary>
        [HttpPost]
        [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
        public async Task<ActionResult<ApplicationRegistrationResultDto>> RegisterApplication([FromBody] RegisterApplicationRequest request)
        {
            var result = await _applicationService.RegisterApplicationAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetApplicationById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// 更新应用程序
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
        public async Task<ActionResult<ApplicationDto>> UpdateApplication(Guid id, [FromBody] UpdateApplicationRequest request)
        {
            if (id.ToString() != request.Id)
            {
                return BadRequest("应用ID不匹配");
            }

            var result = await _applicationService.UpdateApplicationAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// 删除应用程序
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
        public async Task<ActionResult> DeleteApplication(Guid id)
        {
            var result = await _applicationService.DeleteApplicationAsync(id.ToString());
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return NoContent();
        }

        /// <summary>
        /// 重新生成应用程序密钥
        /// </summary>
        [HttpPost("{id}/regenerate-key")]
        [Authorize(Policy = PolicyNames.RequireAdminRole)] // 需要管理员角色
        public async Task<ActionResult<ApplicationRegistrationResultDto>> RegenerateApplicationKey(Guid id)
        {
            var result = await _applicationService.RegenerateApplicationKeyAsync(id.ToString());
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Data);
        }
    }
}