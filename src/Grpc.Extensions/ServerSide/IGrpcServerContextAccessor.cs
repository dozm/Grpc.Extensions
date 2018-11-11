using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.ServerSide
{
    public interface IGrpcServerContextAccessor
    {
        GrpcServerContext Context { get; }
    }
}
