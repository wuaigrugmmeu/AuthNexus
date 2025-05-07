using AuthNexus.Application.Common;

namespace AuthNexus.Application.Applications;

/// <summary>
/// 应用管理服务接口
/// </summary>
public interface IApplicationService
{
    /// <summary>
    /// 注册新应用
    /// </summary>
    Task<ResultDto<ApplicationRegistrationResultDto>> RegisterApplicationAsync(RegisterApplicationRequest request);
    
    /// <summary>
    /// 获取应用信息
    /// </summary>
    Task<ResultDto<ApplicationDto>> GetApplicationAsync(string appUid);
    
    /// <summary>
    /// 更新应用信息
    /// </summary>
    Task<ResultDto<ApplicationDto>> UpdateApplicationAsync(string appUid, UpdateApplicationRequest request);
    
    /// <summary>
    /// 启用应用
    /// </summary>
    Task<ResultDto> EnableApplicationAsync(string appUid);
    
    /// <summary>
    /// 禁用应用
    /// </summary>
    Task<ResultDto> DisableApplicationAsync(string appUid);
    
    /// <summary>
    /// 获取所有应用
    /// </summary>
    Task<ResultDto<IEnumerable<ApplicationDto>>> GetAllApplicationsAsync();
    
    /// <summary>
    /// 验证应用凭据
    /// </summary>
    Task<ResultDto<bool>> ValidateCredentialsAsync(string appUid, string apiKey, string clientSecret);
    
    /// <summary>
    /// 重新生成应用密钥
    /// </summary>
    Task<ResultDto<ApplicationRegistrationResultDto>> RegenerateKeysAsync(Guid id);
}