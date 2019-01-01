using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide
{
    public class DefaultServiceDiscovery : IServiceDiscovery
    {
        private readonly GrpcClientOptions _options;

        public DefaultServiceDiscovery(IOptions<GrpcClientOptions> options)
        {
            _options = options.Value;
        }

        public Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken)
        {
            var clientType = _options.Clients.FirstOrDefault(cm => cm.ServiceName == serviceName)?.ClientType;

            var endpoints = _options.ServiceMaps.Get(clientType);
            if (endpoints == null)
            {
                throw new Exception($"Not found endpoint config of {serviceName}");
            }

            return Task.FromResult(endpoints);
        }
    }
}