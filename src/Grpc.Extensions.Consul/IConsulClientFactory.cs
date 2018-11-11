using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul
{
    public interface IConsulClientFactory
    {
        ConsulClient Create();
    }
}
