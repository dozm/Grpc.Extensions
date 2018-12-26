using Consul;

namespace Grpc.Extensions.Consul
{
    public interface IConsulClientFactory
    {
        ConsulClient Create();
    }
}