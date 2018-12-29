using Grpc.Core;

namespace Grpc.Extensions.ClientSide
{
    public interface IChannelFactory
    {
        Channel Create(ServiceEndPoint serviceEndPoint);
    }
}