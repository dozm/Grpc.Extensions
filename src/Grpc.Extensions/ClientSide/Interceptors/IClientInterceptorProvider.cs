using System.Collections.Generic;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    public interface IClientInterceptorProvider
    {
        IEnumerable<ClientInterceptor> GetInterceptors();
    }
}