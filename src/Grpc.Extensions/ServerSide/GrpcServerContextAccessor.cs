using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.ServerSide
{
    internal class GrpcServerContextAccessor : IGrpcServerContextAccessor
    {
        public GrpcServerContext Context { get; set; }
    }
}
