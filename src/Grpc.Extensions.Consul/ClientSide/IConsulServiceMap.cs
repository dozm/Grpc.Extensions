using System;

namespace Grpc.Extensions.Consul.ClientSide
{
    public interface IConsulServiceMap
    {
        Type GetClientType(string grpcServiceName);
    }
}