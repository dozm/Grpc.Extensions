namespace Grpc.Extensions.ServerSide
{
    public interface IGrpcServerContextAccessor
    {
        GrpcServerContext Context { get; }
    }
}