using System;
using System.Collections.Concurrent;
using System.Linq;
using Grpc.Core;
using Grpc.Extensions.Client.Options;
using Microsoft.Extensions.Logging;

namespace Grpc.Extensions.Client
{
    public class LoadBalancer : ILoadBalancer, IDisposable
    {
        private readonly Lazy<ConcurrentQueue<Channel>> _lazy;
        private readonly ILogger<LoadBalancer> _logger;
        private readonly ServiceEndpoint _serviceEndpoint;

        private ConcurrentQueue<Channel> Pool => _lazy.Value;

        public LoadBalancer(ServiceEndpoint serviceEndpoint, ILogger<LoadBalancer> logger)
        {
            _serviceEndpoint = serviceEndpoint;
            _lazy = new Lazy<ConcurrentQueue<Channel>>(InitQueue, true);
            _logger = logger;
        }

        public Channel GetChannel()
        {
            if (!Pool.TryDequeue(out var channel))
            {
                throw new Exception($"{_serviceEndpoint.ServiceName} 没有可用 channel。");
            }

            switch (channel.State)
            {
                case ChannelState.Shutdown:
                    channel = CreateChannel(channel.Target);
                    break;
                case ChannelState.TransientFailure:
                    channel.ShutdownAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    channel = CreateChannel(channel.Target);
                    break;
                default:
                    break;
            }

            Pool.Enqueue(channel);

            return channel;
        }

        private ConcurrentQueue<Channel> InitQueue()
        {
            return new ConcurrentQueue<Channel>(_serviceEndpoint.Endpoints.Select(CreateChannel));
        }

        private Channel CreateChannel(string endpoint)
        {
            var channel = new Channel(endpoint, ChannelCredentials.Insecure);

            return channel;

        }

        private Channel CheckRecreate(Channel channel)
        {
            switch (channel.State)
            {
                case ChannelState.Shutdown:
                    return CreateChannel(channel.Target);
                case ChannelState.TransientFailure:
                    channel.ShutdownAsync();
                    return CreateChannel(channel.Target);
            }

            return channel;
        }

        public void Dispose()
        {
            if (Pool != null && Pool.Count > 0)
            {
                while (Pool.TryDequeue(out var channel))
                {
                    if (channel.State != ChannelState.Shutdown)
                        channel.ShutdownAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}
