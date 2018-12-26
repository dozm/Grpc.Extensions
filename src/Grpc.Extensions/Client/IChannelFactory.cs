using Grpc.Core;

namespace Grpc.Extensions.Client
{
    public interface IChannelFactory
    {
        Channel Create(ServiceEndPoint serviceEndPoint);
    }
}