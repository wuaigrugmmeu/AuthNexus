using AuthNexus.Domain.Common;

namespace AuthNexus.Domain.Entities;

/// <summary>
/// 权限定义实体，代表应用中的一个具体权限项
/// </summary>
public class PermissionDefinition : Entity
{
    /// <summary>
    /// 所属应用的ID
    /// </summary>
    public Guid ApplicationId { get; private set; }
    
    /// <summary>
    /// 权限代码，在同一应用内唯一
    /// </summary>
    public string Code { get; private set; }
    
    /// <summary>
    /// 权限描述
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; private set; }
    
    /// <summary>
    /// 导航属性：所属应用
    /// </summary>
    public Application Application { get; private set; }

    // EF Core所需的无参构造函数
    private PermissionDefinition() { }

    /// <summary>
    /// 创建新的权限定义
    /// </summary>
    public PermissionDefinition(Guid applicationId, string code, string description)
    {
        if (applicationId == Guid.Empty)
            throw new ArgumentException("应用ID不能为空", nameof(applicationId));
            
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("权限代码不能为空", nameof(code));
        
        ApplicationId = applicationId;
        Code = code;
        Description = description ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新权限定义
    /// </summary>
    public void Update(string description)
    {
        Description = description ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }
}