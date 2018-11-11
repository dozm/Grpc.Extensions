using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.ExecutionStrategies
{
    public interface IExecutionStrategyFactory
    {
        /// <summary>
        ///     Creates a new  <see cref="IExecutionStrategy" />.
        /// </summary>
        /// <returns>An instance of <see cref="IExecutionStrategy" />.</returns>
        IExecutionStrategy Create();
    }
}
