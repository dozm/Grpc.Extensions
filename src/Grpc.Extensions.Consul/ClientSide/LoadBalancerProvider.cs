using Grpc.Extensions.ClientSide;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class LoadBalancerProvider : ILoadBalancerProvider
    {
        public readonly ConcurrentDictionary<string, ILoadBalancer> _loadBalancerMap = new ConcurrentDictionary<string, ILoadBalancer>();
        private IServiceDiscovery _serviceDiscovery;
        private IChannelFactory _channelFactory;
        private ILoggerFactory _loggerFactory;
        private ConsulClientOptions _consulClientOptions;
        private GrpcClientOptions _grpcClientOptions;

        public LoadBalancerProvider(IServiceDiscovery serviceDiscovery, IChannelFactory channelFactory, ILoggerFactory loggerFactory,
            IOptions<ConsulClientOptions> consulClientOptions,
            IOptions<GrpcClientOptions> grpcClientOptions)
        {
            _serviceDiscovery = serviceDiscovery;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
            _consulClientOptions = consulClientOptions.Value;
            _grpcClientOptions = grpcClientOptions.Value;
        }

        public ILoadBalancer GetLoadBalancer(string serviceName)
        {
            var clientType = _grpcClientOptions.Clients.FirstOrDefault(cm => cm.ServiceName == serviceName)?.ClientType;

            var consulServiceName = _consulClientOptions.ServiceMaps.Get(clientType);
            if (consulServiceName == null)
                throw new Exception($"没有找到 {clientType} 所的应用的 consul 服务。");

            return _loadBalancerMap.GetOrAdd(consulServiceName, CreateLoadBalancer);
        }

        private ILoadBalancer CreateLoadBalancer(string consulServiceName)
        {
            return new LoadBalancer(consulServiceName, _serviceDiscovery, _channelFactory, _loggerFactory);
        }
    }
}