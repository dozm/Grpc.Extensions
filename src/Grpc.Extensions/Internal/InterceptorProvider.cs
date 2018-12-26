using Grpc.Core.Interceptors;
using System.Collections.Generic;

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