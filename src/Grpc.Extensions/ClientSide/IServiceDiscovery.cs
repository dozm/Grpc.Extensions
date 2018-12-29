using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide
{
    public interface IServiceDiscovery
    {
        Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken);
    }
}