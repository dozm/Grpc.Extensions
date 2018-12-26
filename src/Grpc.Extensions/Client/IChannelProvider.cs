using Grpc.Core;
using System;

namespace Grpc.Extensions.Client
{
    public interface IChannelProvider
    {
        Channel GetChannel(string grpcServiceName);

        Channel GetChannel(Type clientType);
    }
}