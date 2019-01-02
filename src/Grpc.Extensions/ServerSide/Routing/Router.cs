using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Grpc.Extensions.ServerSide.Routing
{
    // TODO:缓存 Handler 创建委托。
    public class Router
    {
        private readonly GrpcServerOptions _options;
        private readonly IGrpcContextAccessor _grpcContextAccessor;
        private readonly ILogger<Router> _logger;

        public Router(IGrpcContextAccessor grpcContextAccessor, IOptions<GrpcServerOptions> options, ILogger<Router> logger)
        {
            _options = options.Value;
            _grpcContextAccessor = grpcContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> Unary<TRequest, TResponse>(TRequest request, ServerCallContext context)
        {
            var handler = GetHandler(context.Method, _grpcContextAccessor.GrpcContext.ServiceProvider);
            return await (Task<TResponse>)handler.DynamicInvoke(request, context);
        }

        public async Task<TResponse> ClientStreaming<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)
        {
            var handler = GetHandler(context.Method, _grpcContextAccessor.GrpcContext.ServiceProvider);

            return await (Task<TResponse>)handler.DynamicInvoke(requestStream, context);

        }

        public async Task<TResponse> ServerStreaming<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public async Task DuplexStreaming<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        private Delegate GetHandler(string name,IServiceProvider serviceProvider)
        {
            var (serviceName, methodName) = ResolveName(name);
            var serviceMetadata = _options.ServiceMetadatas.FirstOrDefault(m => m.ServiceName == serviceName);
            var handlerType = serviceMetadata.ServiceType;

            var handler = serviceProvider.GetService(handlerType);

            var handleMethod = handlerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            var methodDescriptor = serviceMetadata.Descriptor.Methods.FirstOrDefault(m => m.Name == methodName);
            var handlerDelegateType = ReflectionData.GrpcServerMethodDelegateTypes[methodDescriptor.GetMethodType()].MakeGenericType(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);

            var handlerDelegate = handleMethod.CreateDelegate(handlerDelegateType, handler);

            return handlerDelegate;
        }

        private (string, string) ResolveName(string fullName)
        {
            var arr = fullName.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return (arr[0], arr[1]);
        }
    }
}