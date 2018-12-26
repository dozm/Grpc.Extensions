using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ServerSide
{
    public interface IGrpcServerLifetime
    {
        Task StartedAsync(CancellationToken cancellationToken);

        Task StoppingAsync(CancellationToken cancellationToken);
    }
}