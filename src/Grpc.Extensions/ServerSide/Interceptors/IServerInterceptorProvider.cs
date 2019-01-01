using System.Collections.Generic;

namespace Grpc.Extensions.ServerSide.Interceptors
{
    public interface IServerInterceptorProvider
    {
        IEnumerable<ServerInterceptor> GetInterceptors();
    }
}