using System.Collections.Generic;
using System.Linq;

namespace Grpc.Extensions.ServerSide.Interceptors
{
    public class ServerInterceptorProvider : IServerInterceptorProvider
    {
        private readonly IEnumerable<ServerInterceptor> _interceptors;

        public ServerInterceptorProvider(IEnumerable<ServerInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public IEnumerable<ServerInterceptor> GetInterceptors()
        {
            return _interceptors.OrderBy(i => i.Order);
        }
    }
}