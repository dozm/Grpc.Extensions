using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ExecutionStrategies
{
    public abstract class ExecutionStrategy : IExecutionStrategy
    {
        /// <summary>
        ///     The default number of retry attempts.
        /// </summary>
        protected static readonly int DefaultMaxRetryCount = 6;

        /// <summary>
        ///     The default maximum time delay between retries, must be nonnegative.
        /// </summary>
        protected static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     The default maximum random factor, must not be lesser than 1.
        /// </summary>
        private const double DefaultRandomFactor = 1.1;

        /// <summary>
        ///     The default base for the exponential function used to compute the delay between retries, must be positive.
        /// </summary>
        private const double DefaultExponentialBase = 2;

        /// <summary>
        ///     The default coefficient for the exponential function used to compute the delay between retries, must be nonnegative.
        /// </summary>
        private static readonly TimeSpan _defaultCoefficient = TimeSpan.FromSeconds(1);

        protected ExecutionStrategy(
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            ILogger logger)
        {
            if (maxRetryCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount));
            }

            if (maxRetryDelay.TotalMilliseconds < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetryDelay));
            }

            MaxRetryCount = maxRetryCount;
            MaxRetryDelay = maxRetryDelay;
            _logger = logger;
        }

        /// <summary>
        ///     The list of exceptions that caused the operation to be retried so far.
        /// </summary>
        protected virtual List<Exception> ExceptionsEncountered { get; } = new List<Exception>();

        /// <summary>
        ///     A pseudo-random number generator that can be used to vary the delay between retries.
        /// </summary>
        protected virtual Random Random { get; } = new Random();

        /// <summary>
        ///     The maximum number of retry attempts.
        /// </summary>
        protected virtual int MaxRetryCount { get; }

        /// <summary>
        ///     The maximum delay between retries.
        /// </summary>
        protected virtual TimeSpan MaxRetryDelay { get; }

        private readonly ILogger _logger;
        private static readonly AsyncLocal<bool?> _suspended = new AsyncLocal<bool?>();

        /// <summary>
        ///     Indicates whether the strategy is suspended. The strategy is typically suspending while executing to avoid
        ///     recursive execution from nested operations.
        /// </summary>
        protected static bool Suspended
        {
            get => _suspended.Value ?? false;
            set => _suspended.Value = value;
        }

        /// <summary>
        ///     Indicates whether this <see cref="IExecutionStrategy" /> might retry the execution after a failure.
        /// </summary>
        public virtual bool RetriesOnFailure => !Suspended;

        public virtual TResult Execute<TState, TResult>(
            TState state,
            Func<TState, TResult> operation,
            Func<TState, ExecutionResult<TResult>> verifySucceeded)
        {
            if (Suspended)
            {
                return operation(state);
            }

            OnFirstExecution();

            return ExecuteImplementation(operation, verifySucceeded, state);
        }

        private TResult ExecuteImplementation<TState, TResult>(
            Func<TState, TResult> operation,
            Func<TState, ExecutionResult<TResult>> verifySucceeded,
            TState state)
        {
            while (true)
            {
                TimeSpan? delay;
                try
                {
                    Suspended = true;
                    var result = operation(state);
                    Suspended = false;
                    return result;
                }
                catch (Exception ex)
                {
                    Suspended = false;
                    if (verifySucceeded != null
                        && CallOnWrappedException(ex, ShouldVerifySuccessOn))
                    {
                        var result = ExecuteImplementation(verifySucceeded, null, state);
                        if (result.IsSuccessful)
                        {
                            return result.Result;
                        }
                    }

                    if (!CallOnWrappedException(ex, ShouldRetryOn))
                    {
                        throw;
                    }

                    ExceptionsEncountered.Add(ex);

                    delay = GetNextDelay(ex);
                    if (delay == null)
                    {
                        throw new Exception($"Maximum number of retries ({MaxRetryCount}) exceeded with '{GetType().Name}'. See inner exception for the most recent failure.", ex);
                    }

                    LogRetry(delay.Value, ExceptionsEncountered);
                    OnRetry();
                }

                using (var waitEvent = new ManualResetEventSlim(false))
                {
                    waitEvent.WaitHandle.WaitOne(delay.Value);
                }
            }
        }

        public virtual Task<TResult> ExecuteAsync<TState, TResult>(
            TState state,
            Func<TState, CancellationToken, Task<TResult>> operation,
            Func<TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
            CancellationToken cancellationToken = default)
        {
            if (Suspended)
            {
                return operation(state, cancellationToken);
            }

            OnFirstExecution();
            return ExecuteImplementationAsync(operation, verifySucceeded, state, cancellationToken);
        }

        private async Task<TResult> ExecuteImplementationAsync<TState, TResult>(
            Func<TState, CancellationToken, Task<TResult>> operation,
            Func<TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
            TState state,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                TimeSpan? delay;
                try
                {
                    Suspended = true;
                    var result = await operation(state, cancellationToken);
                    Suspended = false;
                    return result;
                }
                catch (Exception ex)
                {
                    Suspended = false;
                    if (verifySucceeded != null
                        && CallOnWrappedException(ex, ShouldVerifySuccessOn))
                    {
                        var result = await ExecuteImplementationAsync(verifySucceeded, null, state, cancellationToken);
                        if (result.IsSuccessful)
                        {
                            return result.Result;
                        }
                    }

                    if (!CallOnWrappedException(ex, ShouldRetryOn))
                    {
                        throw;
                    }

                    ExceptionsEncountered.Add(ex);

                    delay = GetNextDelay(ex);
                    if (delay == null)
                    {
                        throw new Exception($"Maximum number of retries ({MaxRetryCount}) exceeded with '{GetType().Name}'. See inner exception for the most recent failure.", ex);
                    }
                    LogRetry(delay.Value, ExceptionsEncountered);
                    OnRetry();
                }

                await Task.Delay(delay.Value, cancellationToken);
            }
        }

        /// <summary>
        ///     Method called before the first operation execution
        /// </summary>
        protected virtual void OnFirstExecution()
        {
            ExceptionsEncountered.Clear();
        }

        /// <summary>
        ///     Method called before retrying the operation execution
        /// </summary>
        protected virtual void OnRetry()
        {
        }

        protected virtual TimeSpan? GetNextDelay(Exception lastException)
        {
            var currentRetryCount = ExceptionsEncountered.Count - 1;
            if (currentRetryCount < MaxRetryCount)
            {
                var delta = (Math.Pow(DefaultExponentialBase, currentRetryCount) - 1.0)
                            * (1.0 + Random.NextDouble() * (DefaultRandomFactor - 1.0));

                var delay = Math.Min(
                    _defaultCoefficient.TotalMilliseconds * delta,
                    MaxRetryDelay.TotalMilliseconds);

                return TimeSpan.FromMilliseconds(delay);
            }

            return null;
        }

        protected virtual bool ShouldVerifySuccessOn(Exception exception)
            => ShouldRetryOn(exception);

        protected abstract bool ShouldRetryOn(Exception exception);

        public static TResult CallOnWrappedException<TResult>(
            Exception exception, Func<Exception, TResult> exceptionHandler)
        {
            return exceptionHandler(exception);
        }

        protected virtual void LogRetry(TimeSpan delay, IReadOnlyList<Exception> exceptionsEncountered)
        {
            var exceptionsCount = exceptionsEncountered.Count;
            var lastException = exceptionsEncountered[exceptionsCount - 1];

            _logger.LogError(lastException, string.Empty);
            _logger.LogInformation($"将在延迟 {delay} 后开始第 {exceptionsCount}/{MaxRetryCount} 次重试...\n");
        }
    }
}