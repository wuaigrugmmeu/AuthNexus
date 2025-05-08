using Serilog.Context;

namespace AuthNexus.Api.Middlewares
{
    /// <summary>
    /// 关联ID中间件 - 用于分布式追踪
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 从请求头中获取关联ID，如果没有则生成一个新的
            string? correlationId = context.Request.Headers[CorrelationIdHeaderName];
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers[CorrelationIdHeaderName] = correlationId;
            }

            // 确保响应中也包含相同的关联ID
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeaderName] = correlationId;
                return Task.CompletedTask;
            });

            // 将关联ID添加到日志上下文
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}