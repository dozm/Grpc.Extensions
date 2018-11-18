using Grpc.Core;
using Grpc.Extensions.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Consul;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ChannelProviderWithConsul : IChannelProvider
    {

        public readonly ConcurrentDictionary<string, ILoadBalancer> _poolMap = new ConcurrentDictionary<string, ILoadBalancer>();

        private readonly GrpcClientOptions _grpcClientOptions;
        private readonly ConsulClientOptions _options;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILoggerFactory _loggerFactory;

        public ChannelProviderWithConsul(
            IOptions<ConsulClientOptions> options,
             IOptions<GrpcClientOptions> grpcClientOptions,
            IServiceDiscovery serviceDiscovery,
            ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _grpcClientOptions = grpcClientOptions.Value;
            _serviceDiscovery = serviceDiscovery;
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

            var endpoints = _serviceDiscovery.DiscoverAsync(consulServiceName).GetAwaiter().GetResult();

            return GetChannelFromPool(consulServiceName);
        }



        private Channel CreateChannel(ServiceEndPoint[] endpoints)
        {
            var rand = new Random();

            var target = endpoints[rand.Next(0, endpoints.Length)];

            return new Channel(target.ToString(), ChannelCredentials.Insecure);
        }

        private Type GetClientType(string grpcServiceName)
        {
            var clientType = _grpcClientOptions.Clients.FirstOrDefault(c => c.ServiceName == grpcServiceName)?.ClientType;
            if (clientType == null)
                throw new Exception($"没有找到 grpc 服务 \"{grpcServiceName}\" 所对应的客户端类型。");

            return clientType;
        }

        private Channel GetChannelFromPool(string consulServiceName)
        {

            var pool = _poolMap.GetOrAdd(consulServiceName, CreatePool);

            return pool.GetChannel();
        }

        private ILoadBalancer CreatePool(string consulServiceName)
        {
            
             return new LoadBalancer(consulServiceName, _serviceDiscovery, _loggerFactory);
        }

        HashSet<Channel> ChannelSet = new HashSet<Channel>();

    }
}
