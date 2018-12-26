using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Extensions.Client
{
    public interface ILoadBalancer
    {
        Task<Channel> SelectChannelAsync();

        Channel SelectChannel();
    }
}