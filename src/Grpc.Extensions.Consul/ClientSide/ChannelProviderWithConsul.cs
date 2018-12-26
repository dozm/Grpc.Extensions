using Grpc.Core;
using Grpc.Extensions.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ChannelProviderWithConsul : IChannelProvider
    {
        public readonly ConcurrentDictionary<string, ILoadBalancer> _loadBalancerMap = new ConcurrentDictionary<string, ILoadBalancer>();

        private readonly GrpcClientOptions _grpcClientOptions;
        private readonly ConsulClientOptions _options;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IChannelFactory _channelFactory;
        private readonly ILoggerFactory _loggerFactory;

        public ChannelProviderWithConsul(
            IOptions<ConsulClientOptions> options,
            IOptions<GrpcClientOptions> grpcClientOptions,
            IChannelFactory channelFactory,
            IServiceDiscovery serviceDiscovery,
            ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _grpcClientOptions = grpcClientOptions.Value;
            _serviceDiscovery = serviceDiscovery;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
        }

        public Channel GetChannel(string grpcServiceName)
        {
            return GetChannel(GetClientType(grpcServiceName));
        }

        public Channel GetChannel(Type clientType)
        {
            if (!_options.ServiceMap.TryGetValue(clientType, out var consulServiceName))
            {
                throw new Exception($"没有找到 {clientType} 所的应用的 consul 服务。");
            }

            var loadBalancer = _loadBalancerMap.GetOrAdd(consulServiceName, CreateLoadBalancer);
            return loadBalancer.SelectChannel();
        }

        private Type GetClientType(string grpcServiceName)
        {
            var clientType = _grpcClientOptions.Clients.FirstOrDefault(c => c.ServiceName == grpcServiceName)?.ClientType;
            if (clientType == null)
                throw new Exception($"没有找到 grpc 服务 \"{grpcServiceName}\" 所对应的客户端类型。");

            return clientType;
        }

        private ILoadBalancer CreateLoadBalancer(string consulServiceName)
        {
            return new LoadBalancer(consulServiceName, _serviceDiscovery, _channelFactory, _loggerFactory);
        }
    }
}