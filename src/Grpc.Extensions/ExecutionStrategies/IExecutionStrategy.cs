using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ExecutionStrategies
{
    public interface IExecutionStrategy
    {
        bool RetriesOnFailure { get; }

        TResult Execute<TState, TResult>(
            TState state,
             Func<TState, TResult> operation,
           Func<TState, ExecutionResult<TResult>> verifySucceeded);

        Task<TResult> ExecuteAsync<TState, TResult>(
            TState state,
           Func<TState, CancellationToken, Task<TResult>> operation,
           Func<TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
            CancellationToken cancellationToken = default);
    }
}