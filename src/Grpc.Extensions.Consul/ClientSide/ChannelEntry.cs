using Grpc.Core;
using Grpc.Extensions.Client;
using System;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ChannelEntry : IDisposable
    {
        private bool _disposed;

        public ServiceEndPoint EndPoint { get; }
        public Channel Value { get; }

        public ChannelEntry(Channel channel, ServiceEndPoint endPoint)
        {
            Value = channel;
            EndPoint = endPoint;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed && Value?.State != ChannelState.Shutdown)
            {
                Value.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                _disposed = true;
            }
        }

        ~ChannelEntry()
        {
            Dispose(false);
        }
    }
}