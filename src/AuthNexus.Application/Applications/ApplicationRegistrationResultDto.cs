namespace AuthNexus.Application.Applications;

/// <summary>
/// 应用注册成功后的响应，包含敏感信息
/// </summary>
public class ApplicationRegistrationResultDto
{
    /// <summary>
    /// 应用ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 应用的唯一标识符
    /// </summary>
    public string AppUID { get; set; }
    
    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// API密钥，注册后仅返回一次
    /// </summary>
    public string ApiKey { get; set; }
    
    /// <summary>
    /// 客户端密钥，注册后仅返回一次
    /// </summary>
    public string ClientSecret { get; set; }
}