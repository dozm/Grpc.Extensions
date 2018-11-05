using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public interface IChannelPool
    {
        Channel Rent();

        bool Return(Channel channel);
    }
}
