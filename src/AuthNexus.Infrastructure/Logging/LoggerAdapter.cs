using AuthNexus.SharedKernel.Interfaces;
using Serilog;
using System;

namespace AuthNexus.Infrastructure.Logging
{
    /// <summary>
    /// Serilog日志适配器
    /// </summary>
    public class LoggerAdapter<T> : IAppLogger<T>
    {
        private readonly ILogger _logger;

        public LoggerAdapter()
        {
            _logger = Log.ForContext<T>();
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
    }
}