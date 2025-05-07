using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 应用实体，代表接入权限系统的一个应用
/// </summary>
public class Application : Entity
{
    /// <summary>
    /// 应用的唯一标识符，用于外部引用
    /// </summary>
    public string AppUID { get; private set; }
    
    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 哈希后的API密钥
    /// </summary>
    public string HashedApiKey { get; private set; }
    
    /// <summary>
    /// 哈希后的客户端密钥
    /// </summary>
    public string HashedClientSecret { get; private set; }
    
    /// <summary>
    /// 应用描述
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// 应用是否启用
    /// </summary>
    public bool IsEnabled { get; private set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    // EF Core所需的无参构造函数
    private Application() { }

    /// <summary>
    /// 创建新的应用
    /// </summary>
    public Application(string appUID, string name, string hashedApiKey, string hashedClientSecret, string description)
    {
        if (string.IsNullOrWhiteSpace(appUID))
            throw new ArgumentException("应用标识不能为空", nameof(appUID));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("应用名称不能为空", nameof(name));
            
        if (string.IsNullOrWhiteSpace(hashedApiKey))
            throw new ArgumentException("API密钥不能为空", nameof(hashedApiKey));
            
        if (string.IsNullOrWhiteSpace(hashedClientSecret))
            throw new ArgumentException("客户端密钥不能为空", nameof(hashedClientSecret));
        
        AppUID = appUID;
        Name = name;
        HashedApiKey = hashedApiKey;
        HashedClientSecret = hashedClientSecret;
        Description = description ?? string.Empty;
        IsEnabled = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新应用信息
    /// </summary>
    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("应用名称不能为空", nameof(name));
            
        Name = name;
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 启用应用
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 禁用应用
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新应用密钥
    /// </summary>
    public void UpdateKeys(string hashedApiKey, string hashedClientSecret)
    {
        if (string.IsNullOrWhiteSpace(hashedApiKey))
            throw new ArgumentException("API密钥不能为空", nameof(hashedApiKey));
            
        if (string.IsNullOrWhiteSpace(hashedClientSecret))
            throw new ArgumentException("客户端密钥不能为空", nameof(hashedClientSecret));
            
        HashedApiKey = hashedApiKey;
        HashedClientSecret = hashedClientSecret;
        UpdatedAt = DateTime.UtcNow;
    }
}