using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ClientSide
{
    public interface IConsulServiceMap
    {
        Type GetClientType(string grpcServiceName);
    }
}
