namespace AuthNexus.Api.Middlewares
{
    /// <summary>
    /// 请求日志记录中间件
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 记录请求开始信息
            var requestTime = DateTime.UtcNow;
            _logger.LogInformation(
                "Request started - HTTP {Method} {Path} from {IpAddress}", 
                context.Request.Method, 
                context.Request.Path, 
                context.Connection.RemoteIpAddress);

            try
            {
                // 调用管道中的下一个中间件
                await _next(context);
            }
            finally
            {
                // 记录请求结束信息，包含响应状态码和总耗时
                var elapsed = DateTime.UtcNow - requestTime;
                _logger.LogInformation(
                    "Request completed - HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    elapsed.TotalMilliseconds);
            }
        }
    }
}