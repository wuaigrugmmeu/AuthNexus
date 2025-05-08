using System.Net;
using AuthNexus.SharedKernel.Exceptions;
using AuthNexus.SharedKernel.Models;
using System.Text.Json;

namespace AuthNexus.Api.Middlewares
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

            var statusCode = GetStatusCode(exception);
            var response = new ApiErrorResponse(
                message: GetErrorMessage(exception),
                statusCode: statusCode,
                errors: GetErrors(exception)
            )
            {
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception) => exception switch
        {
            BaseApplicationException baseEx => baseEx.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };

        private string GetErrorMessage(Exception exception)
        {
            // 生产环境下隐藏敏感错误详情
            if (!_environment.IsDevelopment() && exception is not BaseApplicationException)
            {
                return "发生了内部服务器错误";
            }

            return exception.Message;
        }

        private static IEnumerable<string>? GetErrors(Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                return validationException.Errors.SelectMany(e => e.Value);
            }

            return null;
        }
    }
}