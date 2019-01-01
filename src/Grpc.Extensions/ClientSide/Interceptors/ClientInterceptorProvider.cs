using System.Collections.Generic;
using System.Linq;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    public class ClientInterceptorProvider : IClientInterceptorProvider
    {
        private readonly IEnumerable<ClientInterceptor> _interceptors;

        public ClientInterceptorProvider(IEnumerable<ClientInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IEnumerable<ClientInterceptor> GetInterceptors()
        {
            return _interceptors.OrderBy(i => i.Order);
        }
    }
}