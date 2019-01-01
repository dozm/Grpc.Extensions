using Microsoft.Extensions.Logging;
using System;

namespace Sample.Grpc.Server
{
    public class A : IDisposable
    {
        private readonly ILogger<A> _logger;
        private bool disposed;

        public A(ILogger<A> logger)
        {
            _logger = logger;
        }

        public void Test()
        {
            if (disposed)
                throw new ObjectDisposedException("已释放的对象");

            _logger.LogInformation("正常 A");
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                _logger.LogInformation($"释放 A");
            }
        }
    }
}