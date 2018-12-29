using Grpc.Core;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Grpc.Extensions.ClientSide
{
    public class ChannelProvider : IChannelProvider
    {
        private readonly GrpcClientOptions _options;
        private readonly ILoadBalancerProvider _loadBalancerProvider;

        public ChannelProvider(IOptions<GrpcClientOptions> options,
            ILoadBalancerProvider loadBalancerProvider)
        {
            _options = options.Value;
            _loadBalancerProvider = loadBalancerProvider;
        }

        public Channel GetChannel(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentException($"Parameter {nameof(serviceName)} cannot be null.");

            return _loadBalancerProvider.GetLoadBalancer(serviceName).SelectChannel();
        }

        public Channel GetChannel(Type clientType)
        {
            if (clientType == null)
                throw new ArgumentException($"Parameter {nameof(clientType)} cannot be null.");

            var serviceName = _options.Clients.FirstOrDefault(cm => cm.ClientType == clientType)?.ServiceName;
            return GetChannel(serviceName);
        }
    }
}