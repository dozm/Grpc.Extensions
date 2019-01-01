using Consul;
using Grpc.Extensions.ClientSide;
using Grpc.Extensions.ExecutionStrategies;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly ConsulClientOptions _options;
        private readonly IExecutionStrategyFactory _executionStrategyFactory;

        public ConsulServiceDiscovery(IOptions<ConsulClientOptions> options,
            IExecutionStrategyFactory executionStrategyFactory)
        {
            _options = options.Value;
            _executionStrategyFactory = executionStrategyFactory;
        }

        public async Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            return await _executionStrategyFactory.Create()
                .ExecuteAsync(serviceName, DiscoverImplementAsync, null);
        }

        private async Task<ServiceEndPoint[]> DiscoverImplementAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            using (var consul = CreateConsulClient())
            {
                var serviceQuery = await consul.Health.Service(serviceName);
                var endpoints = serviceQuery.Response.Select(se => se.Service).Select(service =>
                {
                    return new ServiceEndPoint { Host = service.Address, Port = service.Port };
                });

                if (endpoints == null || endpoints.Count() == 0)
                {
                    throw new Exception($"未发现可用的服务终端:{serviceName}");
                }

                return endpoints.ToArray();
            }
        }

        private ConsulClient CreateConsulClient()
        {
            return new ConsulClient(config =>
            {
                config.Address = new Uri(_options.Address);
            });
        }
    }
}