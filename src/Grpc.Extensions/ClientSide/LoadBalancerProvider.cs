using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Grpc.Extensions.ClientSide
{
    public class LoadBalancerProvider : ILoadBalancerProvider
    {
        public readonly ConcurrentDictionary<string, ILoadBalancer> _loadBalancerMap = new ConcurrentDictionary<string, ILoadBalancer>();
        private IServiceDiscovery _serviceDiscovery;
        private IChannelFactory _channelFactory;
        private ILoggerFactory _loggerFactory;

        public LoadBalancerProvider(IServiceDiscovery serviceDiscovery, IChannelFactory channelFactory, ILoggerFactory loggerFactory)
        {
            _serviceDiscovery = serviceDiscovery;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
        }

        public ILoadBalancer GetLoadBalancer(string serviceName)
        {
            return _loadBalancerMap.GetOrAdd(serviceName, CreateLoadBalancer);
        }

        private ILoadBalancer CreateLoadBalancer(string serviceName)
        {
            return new LoadBalancer(serviceName, _serviceDiscovery, _channelFactory, _loggerFactory);
        }
    }
}