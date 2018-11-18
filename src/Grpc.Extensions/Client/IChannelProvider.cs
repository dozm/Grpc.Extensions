using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public interface IChannelProvider
    {
        Channel GetChannel(string grpcServiceName);
        Channel GetChannel(Type clientType);
    }
}
