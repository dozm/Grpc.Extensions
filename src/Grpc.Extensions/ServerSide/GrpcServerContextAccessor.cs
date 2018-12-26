namespace Grpc.Extensions.ServerSide
{
    internal class GrpcServerContextAccessor : IGrpcServerContextAccessor
    {
        public GrpcServerContext Context { get; set; }
    }
}