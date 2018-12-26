namespace Grpc.Extensions.Client
{
    public interface IServiceEndpointProvider
    {
        string GetEndpoint(string serviceName);
    }
}