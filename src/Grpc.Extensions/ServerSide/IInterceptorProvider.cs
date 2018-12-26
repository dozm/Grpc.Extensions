using Grpc.Core.Interceptors;
using System.Collections.Generic;

namespace Grpc.Extensions
{
    public interface IInterceptorProvider
    {
        IEnumerable<Interceptor> GetInterceptors();
    }
}