using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public interface IServiceEndpointProvider
    {
        string GetEndpoint(string serviceName);
    }
}
