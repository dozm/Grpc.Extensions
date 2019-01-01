using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    internal class LoggableAsyncStreamReader<T> : IAsyncStreamReader<T>
    {
        private readonly IAsyncStreamReader<T> _reader;
        private readonly ILogger _logger;

        public LoggableAsyncStreamReader(IAsyncStreamReader<T> reader, ILogger logger)
        {
            _reader = reader;
            _logger = logger;
        }

        public T Current => _reader.Current;

        public void Dispose()
        {
            _reader.Dispose();
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            try
            {
                return await _reader.MoveNext(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw ex;
            }
        }
    }
}