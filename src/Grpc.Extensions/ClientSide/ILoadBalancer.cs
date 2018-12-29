using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide
{
    public interface ILoadBalancer
    {
        Task<Channel> SelectChannelAsync();

        Channel SelectChannel();
    }
}