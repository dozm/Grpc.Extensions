namespace Grpc.Extensions.ClientSide
{
    public interface ILoadBalancerProvider
    {
        ILoadBalancer GetLoadBalancer(string serviceName);
    }
}