using Grpc.Core;

namespace Grpc.Extensions.ServerSide.Routing
{
    public class RoutingServiceBinder : ServiceBinderBase
    {
        private readonly Router _router;
        private ServerServiceDefinition.Builder _builder = ServerServiceDefinition.CreateBuilder();

        public RoutingServiceBinder(Router router)
        {
            _router = router;
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            _builder.AddMethod(method, _router.Unary<TRequest, TResponse>);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
        {
            _builder.AddMethod(method, _router.ClientStreaming<TRequest, TResponse>);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
        {
            _builder.AddMethod(method, _router.ServerStreaming<TRequest, TResponse>);
        }

        public override void AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
        {
            _builder.AddMethod(method, _router.DuplexStreaming<TRequest, TResponse>);
        }
    }
}