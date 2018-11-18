using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Client
{
    public interface IServiceDiscovery
    {
        Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken);
    }
}
