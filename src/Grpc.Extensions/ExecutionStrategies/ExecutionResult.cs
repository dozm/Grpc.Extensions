// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Grpc.Extensions.ExecutionStrategies
{
    public class ExecutionResult<TResult>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ExecutionResult{TResult}" />.
        /// </summary>
        /// <param name="successful"><c>true</c> if the operation succeeded.</param>
        /// <param name="result">The result of the operation if successful.</param>
        public ExecutionResult(bool successful, TResult result)
        {
            IsSuccessful = successful;
            Result = result;
        }

        /// <summary>
        ///     Indicates whether the operation succeeded.
        /// </summary>
        public virtual bool IsSuccessful { get; }

        /// <summary>
        ///     The result of the operation if successful.
        /// </summary>
        public virtual TResult Result { get; }
    }
}