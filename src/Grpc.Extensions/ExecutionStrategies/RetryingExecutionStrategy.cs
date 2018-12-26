using Microsoft.Extensions.Logging;
using System;

namespace Grpc.Extensions.ExecutionStrategies
{
    /// <summary>
    /// 任何异常都执行重试。
    /// </summary>
    public class RetryingExecutionStrategy : ExecutionStrategy
    {
        public RetryingExecutionStrategy(int maxRetryCount, TimeSpan maxDelay, ILogger<RetryingExecutionStrategy> logger) : base(maxRetryCount, maxDelay, logger)
        { }

        public RetryingExecutionStrategy(ILogger<RetryingExecutionStrategy> logger)
            : this(DefaultMaxRetryCount, DefaultMaxDelay, logger)
        { }

        public RetryingExecutionStrategy(int maxRetryCount, ILogger<RetryingExecutionStrategy> logger)
            : this(maxRetryCount, DefaultMaxDelay, logger)
        { }

        public RetryingExecutionStrategy(TimeSpan maxDelay, ILogger<RetryingExecutionStrategy> logger)
            : this(DefaultMaxRetryCount, maxDelay, logger)
        { }

        protected override bool ShouldRetryOn(Exception exception)
        {
            return true;
        }
    }
}