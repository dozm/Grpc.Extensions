using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grpc.Extensions.ServerSide.Interceptors
{
    public class CreateContextInterceptor : ServerInterceptor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IGrpcContextAccessor _grpcContextAccessor;

        private readonly ILogger _logger;

        public override int Order { get; set; } = -90;

        public CreateContextInterceptor(IServiceScopeFactory serviceScopeFactory, IGrpcContextAccessor grpcContextAccessor, ILogger<CreateContextInterceptor> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _grpcContextAccessor = grpcContextAccessor;
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            using (var disposer = CreateContext(context))
            {
                return await continuation(request, context);
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var disposer = CreateContext(context))
            {
                return await continuation(requestStream, context);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var disposer = CreateContext(context))
            {
                await continuation(requestStream, responseStream, context);
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            using (var disposer = CreateContext(context))
            {
                await continuation(request, responseStream, context);
            }
        }

        private IDisposable CreateContext(ServerCallContext context)
        {
            _grpcContextAccessor.GrpcContext = new GrpcContext
            {
                ServerCallContext = context,
                ServiceScope = _serviceScopeFactory.CreateScope()
            };

            return Disposable.Create(() =>
            {
                _grpcContextAccessor.GrpcContext?.ServiceScope.Dispose();
                _grpcContextAccessor.GrpcContext = null;
            });
        }
    }
}