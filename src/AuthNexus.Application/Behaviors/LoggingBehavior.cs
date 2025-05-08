using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AuthNexus.Application.Behaviors
{
    /// <summary>
    /// 日志记录行为 - 用于在处理请求前后记录日志
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestGuid = Guid.NewGuid().ToString();

            _logger.LogInformation("[START] {RequestName} {RequestGuid}", requestName, requestGuid);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("[END] {RequestName} {RequestGuid}; Execution time={ElapsedMs}ms", 
                    requestName, requestGuid, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[ERROR] {RequestName} {RequestGuid}; Execution time={ElapsedMs}ms", 
                    requestName, requestGuid, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}