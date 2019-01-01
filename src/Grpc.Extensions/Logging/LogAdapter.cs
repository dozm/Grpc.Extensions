using Microsoft.Extensions.Logging;
using System;

namespace Grpc.Extensions.Logging
{
    public class LogAdapter : Core.Logging.ILogger
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _categoryName;
        private ILogger _logger;

        public LogAdapter(ILoggerFactory loggerFactory, string categoryName = "Grpc.Core")
        {
            _loggerFactory = loggerFactory;
            _categoryName = categoryName;
            _logger = _loggerFactory.CreateLogger(categoryName);
        }

        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            _logger.LogDebug(format, formatArgs);
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            _logger.LogError(format, formatArgs);
        }

        public void Error(Exception exception, string message)
        {
            _logger.LogError(exception, message);
        }

        public Core.Logging.ILogger ForType<T>()
        {
            var categoryName = typeof(T).FullName;
            if (_categoryName == categoryName)
            {
                return this;
            }
            return new LogAdapter(_loggerFactory, categoryName);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            _logger.LogInformation(format, formatArgs);
        }

        public void Warning(string message)
        {
            _logger.LogWarning(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            _logger.LogWarning(format, formatArgs);
        }

        public void Warning(Exception exception, string message)
        {
            _logger.LogWarning(exception, message);
        }
    }
}