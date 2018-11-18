using Grpc.Core;
using Grpc.Extensions.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Grpc.Extensions.Consul.ClientSide
{
    /// <summary>
    ///  每个 consul 服务 对应一个 Channl Pool
    /// </summary>
    public class LoadBalancer : ILoadBalancer
    {
        private ConcurrentQueue<Channel> Pool { get; set; } = new ConcurrentQueue<Channel>();

        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILogger<LoadBalancer> _logger;

        public string ConsulServiceName { get; }

        public LoadBalancer(string consulServiceName, IServiceDiscovery serviceDiscovery, ILoggerFactory loggerFactory)
        {
            ConsulServiceName = consulServiceName;
            _serviceDiscovery = serviceDiscovery;
            _logger = loggerFactory.CreateLogger<LoadBalancer>();
        }

        public Channel GetChannel()
        {
            _logger.LogInformation("获取通道...");
            if (Pool.TryDequeue(out var channel))
            {
                Pool.Enqueue(channel);

                _logger.LogInformation($"通道状态：{channel.State}  {channel.ResolvedTarget}");

                //if (channel.State == ChannelState.Shutdown)
                //{
                //    UpdatePool();
                //    return Rent();
                //}

                return channel;
            }


            UpdatePool();

            return GetChannel();


        }

        private void UpdatePool()
        {
            var endports = _serviceDiscovery.DiscoverAsync(ConsulServiceName).GetAwaiter().GetResult();

            if (endports.Length == 0)
            {
                throw new Exception($"未发现可用的服务:{ConsulServiceName}");
            }
            _logger.LogInformation($"服务数量：{endports.Count()}");

            Pool.Enqueue(new Channel($"dns:///testsvc.service.consul", ChannelCredentials.Insecure));

          
            //var pool = new ConcurrentQueue<Channel>();
            //foreach (var channel in endports.Select(ep => new Channel(ep.ToString(), ChannelCredentials.Insecure)))
            //{
            //    pool.Enqueue(channel);
            //}

            //Pool = pool;
        }
    }
}
