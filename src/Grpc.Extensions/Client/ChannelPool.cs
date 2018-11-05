using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Grpc.Core;

namespace Grpc.Extensions.Client
{
    public class ChannelPool<TClient> : IChannelPool
    {
        private const int DefaultPoolSize = 32;

        private readonly ConcurrentQueue<Channel> _pool = new ConcurrentQueue<Channel>();

        private readonly Func<Channel> _activator;

        private int _maxSize;
        private int _count;
        public ChannelPool()
        {

        }

        public Channel Rent()
        {
            if (_pool.TryDequeue(out var channel))
            {
                Interlocked.Decrement(ref _count);

                Debug.Assert(_count >= 0);

                return channel;
            }

            channel = _activator();

            return channel;
        }

        public bool Return(Channel channel)
        {
            throw new NotImplementedException();
        }
    }
}
