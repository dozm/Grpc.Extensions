using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Grpc.Extensions.Client
{
    public class ChannelProvider : IChannelProvider
    {
        private readonly GrpcClientOptions _options;
        private readonly ILoggerFactory _loggerFactory;
        private ConcurrentDictionary<string, ILoadBalancer> _channelPools = new ConcurrentDictionary<string, ILoadBalancer>();

        public ChannelProvider(IOptions<GrpcClientOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerFactory = loggerFactory;
        }

        public Channel GetChannel(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentException($"Parameter {nameof(serviceName)} cannot be null.");

            var channelPool = _channelPools.GetOrAdd(serviceName, CreateChannelPool);
            var channel = channelPool.SelectChannel();

            return channel;
        }

        public Channel GetChannel(Type clientType)
        {
            var serviceName = _options.Clients.FirstOrDefault(cm => cm.ClientType == clientType)?.ServiceName;

            return GetChannel(serviceName);
        }

        private ILoadBalancer CreateChannelPool(string serviceName)
        {
            if (!_options.ServicesEndpoint.TryGetValue(serviceName, out var serviceEndpoint))
            {
                throw new Exception($"Not found endpoint config of {serviceName}");
            }

            return new LoadBalancer(serviceEndpoint, _loggerFactory.CreateLogger<LoadBalancer>());
        }
    }
}