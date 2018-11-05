using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Extensions.Client
{
    public class PoolableChannel : Channel, IDisposable
    {
        private IChannelPool _channelPool;
        public PoolableChannel(string target, ChannelCredentials credentials) : base(target, credentials)
        { }

        public PoolableChannel(string target, ChannelCredentials credentials, IEnumerable<ChannelOption> options)
          : base(target, credentials, options)
        { }

        public PoolableChannel(string host, int port, ChannelCredentials credentials)
            : base(host, port, credentials)
        { }

        public PoolableChannel(string host, int port, ChannelCredentials credentials, IEnumerable<ChannelOption> options)
        : base(host, port, credentials, options)
        { }

        public virtual void SetPool(IChannelPool channelPool) => _channelPool = channelPool;

        public virtual void Dispose()
        {
            if (State == ChannelState.Shutdown)
            {
                _channelPool = null;
                return;
            }

            if (State == ChannelState.TransientFailure)
            {
                ShutdownAsync().GetAwaiter().GetResult();
                _channelPool = null;
                return;
            }

            if (_channelPool != null)
            {
                _channelPool.Return(this);
                _channelPool = null;
            }
            else
            {
                ShutdownAsync().GetAwaiter().GetResult();
            }
        }
    }
}
