namespace AuthNexus.Application.Applications;

/// <summary>
/// 应用DTO，用于返回应用信息
/// </summary>
public class ApplicationDto
{
    /// <summary>
    /// 应用ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 应用的唯一标识符，用于外部引用
    /// </summary>
    public string AppUID { get; set; }
    
    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 应用描述
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// 应用是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}