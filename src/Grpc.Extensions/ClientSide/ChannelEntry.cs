using Grpc.Core;
using System;

namespace Grpc.Extensions.ClientSide
{
    public class ChannelEntry : IDisposable
    {
        private bool _disposed;

        public ServiceEndPoint EndPoint { get; }
        public Channel Channel { get; }

        public ChannelEntry(Channel channel, ServiceEndPoint endPoint)
        {
            Channel = channel;
            EndPoint = endPoint;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed && Channel?.State != ChannelState.Shutdown)
            {
                Channel.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                _disposed = true;
            }
        }

        ~ChannelEntry()
        {
            Dispose(false);
        }
    }
}