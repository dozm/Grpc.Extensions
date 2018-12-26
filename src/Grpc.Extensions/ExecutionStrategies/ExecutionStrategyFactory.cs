using Microsoft.Extensions.Logging;

namespace Grpc.Extensions.ExecutionStrategies
{
    public class ExecutionStrategyFactory : IExecutionStrategyFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExecutionStrategyFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IExecutionStrategy Create()
        {
            return new RetryingExecutionStrategy(_loggerFactory.CreateLogger<RetryingExecutionStrategy>());
        }
    }
}