using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    public class LogableClientStreamWriter<T> : IClientStreamWriter<T>
    {
        private readonly IClientStreamWriter<T> _writer;
        private readonly ILogger _logger;

        public WriteOptions WriteOptions
        {
            get
            {
                return _writer.WriteOptions;
            }
            set
            {
                _writer.WriteOptions = value;
            }
        }

        public LogableClientStreamWriter(IClientStreamWriter<T> writer, ILogger logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task CompleteAsync()
        {
            await _writer.CompleteAsync();
        }

        public async Task WriteAsync(T message)
        {
            try
            {
                await _writer.WriteAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw ex;
            }
        }
    }
}