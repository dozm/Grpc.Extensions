using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ExecutionStrategies
{
    public static class ExecutionStrategyExtensions
    {
        public static void Execute(
             this IExecutionStrategy strategy,
             Action operation)
        {
            strategy.Execute(
                operation, operationScoped =>
                {
                    operationScoped();
                    return true;
                });
        }

        public static TResult Execute<TResult>(
             this IExecutionStrategy strategy,
             Func<TResult> operation)
        {
            return strategy.Execute(operation, operationScoped => operationScoped());
        }

        public static void Execute<TState>(
             this IExecutionStrategy strategy,
             TState state,
             Action<TState> operation)
        {
            strategy.Execute(
                new
                {
                    operation,
                    state
                }, s =>
                {
                    s.operation(s.state);
                    return true;
                });
        }

        public static Task ExecuteAsync(
             this IExecutionStrategy strategy,
             Func<Task> operation)
        {
            return strategy.ExecuteAsync(
                operation, async (operationScoped, ct) =>
                {
                    await operationScoped();
                    return true;
                }, default);
        }

        public static Task ExecuteAsync(
             this IExecutionStrategy strategy,
             Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken)
        {
            return strategy.ExecuteAsync(
                operation, async (operationScoped, ct) =>
                {
                    await operationScoped(ct);
                    return true;
                }, cancellationToken);
        }

        public static Task<TResult> ExecuteAsync<TResult>(
             this IExecutionStrategy strategy,
             Func<Task<TResult>> operation)
        {
            return strategy.ExecuteAsync(operation, (operationScoped, ct) => operationScoped(), default);
        }

        public static Task<TResult> ExecuteAsync<TResult>(
             this IExecutionStrategy strategy,
             Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken)
        {
            return strategy.ExecuteAsync(operation, (operationScoped, ct) => operationScoped(ct), cancellationToken);
        }

        public static Task ExecuteAsync<TState>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, Task> operation)
        {
            return strategy.ExecuteAsync(
                new
                {
                    operation,
                    state
                }, async (t, ct) =>
                {
                    await t.operation(t.state);
                    return true;
                }, default);
        }

        public static Task ExecuteAsync<TState>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, CancellationToken, Task> operation,
            CancellationToken cancellationToken)
        {
            return strategy.ExecuteAsync(
                new
                {
                    operation,
                    state
                }, async (t, ct) =>
                {
                    await t.operation(t.state, ct);
                    return true;
                }, cancellationToken);
        }

        public static Task<TResult> ExecuteAsync<TState, TResult>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, Task<TResult>> operation)
        {
            return strategy.ExecuteAsync(
                new
                {
                    operation,
                    state
                }, (t, ct) => t.operation(t.state), default);
        }

        public static TResult Execute<TState, TResult>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, TResult> operation)
            => strategy.Execute(operation, verifySucceeded: null, state: state);

        public static Task<TResult> ExecuteAsync<TState, TResult>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken)
            => strategy.ExecuteAsync(state, operation, verifySucceeded: null, cancellationToken: cancellationToken);

        public static TResult Execute<TState, TResult>(
             this IExecutionStrategy strategy,
             Func<TState, TResult> operation,
             Func<TState, ExecutionResult<TResult>> verifySucceeded,
             TState state)
        {
            return strategy.Execute(
                           state,
                           (s) => operation(s),
                           verifySucceeded == null ? (Func<TState, ExecutionResult<TResult>>)null : (s) => verifySucceeded(s));
        }

        public static Task<TResult> ExecuteAsync<TState, TResult>(
             this IExecutionStrategy strategy,
             TState state,
             Func<TState, CancellationToken, Task<TResult>> operation,
             Func<TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
            CancellationToken cancellationToken = default)
        {
            return strategy.ExecuteAsync(
                           state,
                           (s, ct) => operation(s, ct),
                           verifySucceeded == null
                               ? (Func<TState, CancellationToken, Task<ExecutionResult<TResult>>>)null
                               : (s, ct) => verifySucceeded(s, ct), cancellationToken);
        }

        private class ExecutionState<TState, TResult>
        {
            public ExecutionState(
                Func<TState, TResult> operation,
                Func<TState, bool> verifySucceeded,
                TState state)
            {
                Operation = operation;
                VerifySucceeded = verifySucceeded;
                State = state;
            }

            public Func<TState, TResult> Operation { get; }
            public Func<TState, bool> VerifySucceeded { get; }
            public TState State { get; }
            public TResult Result { get; set; }
            public bool CommitFailed { get; set; }
        }

        private class ExecutionStateAsync<TState, TResult>
        {
            public ExecutionStateAsync(
                Func<TState, CancellationToken, Task<TResult>> operation,
                Func<TState, CancellationToken, Task<bool>> verifySucceeded,
                TState state)
            {
                Operation = operation;
                VerifySucceeded = verifySucceeded;
                State = state;
            }

            public Func<TState, CancellationToken, Task<TResult>> Operation { get; }
            public Func<TState, CancellationToken, Task<bool>> VerifySucceeded { get; }
            public TState State { get; }
            public TResult Result { get; set; }
            public bool CommitFailed { get; set; }
        }
    }
}