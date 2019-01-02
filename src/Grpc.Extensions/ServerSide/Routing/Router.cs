using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Extensions.ServerSide.Routing
{
    // TODO:缓存 Handler 创建委托。
    public class Router
    {
        private readonly GrpcServerOptions _options;
        private readonly IGrpcContextAccessor _grpcContextAccessor;
        private readonly ILogger<Router> _logger;
        private readonly IRoutingTable _routingTable;

        private IServiceProvider ServiceProvider => _grpcContextAccessor.GrpcContext.ServiceProvider;

        public Router(IGrpcContextAccessor grpcContextAccessor, IOptions<GrpcServerOptions> options, ILogger<Router> logger, IRoutingTable routingTable)
        {
            _options = options.Value;
            _grpcContextAccessor = grpcContextAccessor;
            _logger = logger;
            _routingTable = routingTable;
        }

        public async Task<TResponse> Unary<TRequest, TResponse>(TRequest request, ServerCallContext context)
        {
            var executor = _routingTable.GetExecutor(context.Method);
            return (TResponse)(await executor.ExecuteAsync(CreateServiceInstance(context), new object[] { request, context }));
        }

        public async Task<TResponse> ClientStreaming<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)
        {
            var executor = _routingTable.GetExecutor(context.Method);
            return (TResponse)(await executor.ExecuteAsync(CreateServiceInstance(context), new object[] { requestStream, context }));
        }

        public async Task ServerStreaming<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)
        {
            var executor = _routingTable.GetExecutor(context.Method);
            await executor.ExecuteAsync(CreateServiceInstance(context), new object[] { request, responseStream, context });
        }

        public async Task DuplexStreaming<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)
        {
            var executor = _routingTable.GetExecutor(context.Method);
            await executor.ExecuteAsync(CreateServiceInstance(context), new object[] { requestStream, responseStream, context });
        }

        private (string, string) ResolveName(string fullName)
        {
            var arr = fullName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return (arr[0], arr[1]);
        }

        private object CreateServiceInstance(ServerCallContext context)
        {
            var (serviceName, methodName) = ResolveName(context.Method);
            var serviceMetadata = _options.ServiceMetadatas.FirstOrDefault(m => m.ServiceName == serviceName);
            var handlerType = serviceMetadata.ServiceType;
            return ServiceProvider.GetRequiredService(handlerType);
        }
    }
}