namespace AuthNexus.SharedKernel.Models
{
    /// <summary>
    /// 通用API错误响应结构
    /// </summary>
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public string? TraceId { get; set; }

        public ApiErrorResponse(string message, int statusCode, IEnumerable<string>? errors = null)
        {
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}