namespace AuthNexus.Application.Common;

/// <summary>
/// 表示操作结果的通用DTO
/// </summary>
/// <typeparam name="T">结果数据类型</typeparam>
public class ResultDto<T>
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool IsSuccess { get; private set; }
    
    /// <summary>
    /// 错误消息（如果操作失败）
    /// </summary>
    public string ErrorMessage { get; private set; }
    
    /// <summary>
    /// 结果数据（如果操作成功）
    /// </summary>
    public T Data { get; private set; }
    
    private ResultDto(bool isSuccess, T data, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }
    
    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static ResultDto<T> Success(T data)
    {
        return new ResultDto<T>(true, data, null);
    }
    
    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static ResultDto<T> Failure(string errorMessage)
    {
        return new ResultDto<T>(false, default, errorMessage);
    }
}

/// <summary>
/// 不包含数据的操作结果DTO
/// </summary>
public class ResultDto
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool IsSuccess { get; private set; }
    
    /// <summary>
    /// 错误消息（如果操作失败）
    /// </summary>
    public string ErrorMessage { get; private set; }
    
    private ResultDto(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
    
    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static ResultDto Success()
    {
        return new ResultDto(true, null);
    }
    
    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static ResultDto Failure(string errorMessage)
    {
        return new ResultDto(false, errorMessage);
    }
}