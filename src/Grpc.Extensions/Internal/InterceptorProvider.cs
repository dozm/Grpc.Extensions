using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core.Interceptors;

namespace Grpc.Extensions.Internal
{
    internal class InterceptorProvider : IInterceptorProvider
    {
        private readonly IEnumerable<Interceptor> _interceptors;

        public InterceptorProvider(IEnumerable<Interceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IEnumerable<Interceptor> GetInterceptors()
        {
            return _interceptors;
        }
    }
}
