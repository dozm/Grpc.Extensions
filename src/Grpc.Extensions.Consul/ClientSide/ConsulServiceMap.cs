using System;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ConsulServiceMap : IConsulServiceMap
    {
        public ConsulServiceMap()
        {
        }

        public string GetClientType(string grpcServiceName)
        {
            throw new NotImplementedException();
        }

        Type IConsulServiceMap.GetClientType(string grpcServiceName)
        {
            throw new NotImplementedException();
        }
    }
}