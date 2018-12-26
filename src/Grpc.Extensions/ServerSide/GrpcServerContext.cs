using Grpc.Core;

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