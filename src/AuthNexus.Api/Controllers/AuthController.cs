using AuthNexus.Application.Features.Authentication.Commands.Login;
using AuthNexus.Application.Features.Authentication.Commands.RegisterUser;
using AuthNexus.Application.Features.Authentication.Dtos;
using AuthNexus.Application.Features.Authentication.Queries.GetMyProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthNexus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>包含访问令牌的结果</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode ?? 500, new { Message = result.Error });
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>包含用户信息的结果</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            var command = new RegisterUserCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetMyProfile), result.Data);
            }

            return StatusCode(result.ErrorCode ?? 500, new { Message = result.Error });
        }

        /// <summary>
        /// 获取当前用户个人资料
        /// </summary>
        /// <returns>包含用户信息的结果</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var query = new GetMyProfileQuery();
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return StatusCode(result.ErrorCode ?? 500, new { Message = result.Error });
        }
    }
}