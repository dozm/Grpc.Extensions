using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public interface IInterceptorProvider
    {
        IEnumerable<Interceptor> GetInterceptors();
    }
}
