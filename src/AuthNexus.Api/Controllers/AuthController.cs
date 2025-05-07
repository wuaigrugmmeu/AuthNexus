using AuthNexus.Application.Applications;
using AuthNexus.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthNexus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IConfiguration _configuration;

    public AuthController(IApplicationService applicationService, IConfiguration configuration)
    {
        _applicationService = applicationService;
        _configuration = configuration;
    }

    [HttpPost("token")]
    public async Task<ActionResult<string>> GetToken(ApplicationAuthRequest request)
    {
        var appResult = await _applicationService.GetApplicationAsync(request.AppUID);
        if (!appResult.IsSuccess)
        {
            return Unauthorized(ResultDto.Failure("应用不存在"));
        }

        // 验证凭据
        var validationResult = await _applicationService.ValidateCredentialsAsync(request.AppUID, request.ApiKey, request.ClientSecret);
        if (!validationResult.IsSuccess || !validationResult.Data)
        {
            return Unauthorized(ResultDto.Failure("无效的应用凭据"));
        }

        // 生成JWT令牌
        var token = GenerateJwtToken(appResult.Data);
        return Ok(new { token });
    }

    private string GenerateJwtToken(ApplicationDto app)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, app.AppUID),
            new Claim("appId", app.Id.ToString()),
            new Claim("appName", app.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class ApplicationAuthRequest
{
    public string AppUID { get; set; }
    public string ApiKey { get; set; }
    public string ClientSecret { get; set; }
}