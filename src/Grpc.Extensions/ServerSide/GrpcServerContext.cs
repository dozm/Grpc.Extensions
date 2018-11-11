using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.ServerSide
{
    public class GrpcServerContext
    {
        public Server GrpcServer { get; }

        public GrpcServerContext(Server server)
        {
            GrpcServer = server;
        }

    }
}
