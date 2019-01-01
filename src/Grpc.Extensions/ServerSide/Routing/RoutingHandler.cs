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
    public class RoutingHandler
    {
        private readonly GrpcServerOptions _options;
        private readonly IGrpcContextAccessor _grpcContextAccessor;
        private readonly ILogger<RoutingHandler> _logger;

        public RoutingHandler(IGrpcContextAccessor grpcContextAccessor, IOptions<GrpcServerOptions> options, ILogger<RoutingHandler> logger)
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

        private Delegate GetHandler(string name, IServiceProvider serviceProvider)
        {
            var (serviceName, methodName) = ResolveName(name);
            var serviceMetadata = _options.ServiceMetadatas.FirstOrDefault(m => m.ServiceName == serviceName);
            var handlerType = serviceMetadata.ServiceType;

            var handler = serviceProvider.GetService(handlerType);

            var handleMethod = handlerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            var methodDescriptor = serviceMetadata.Descriptor.Methods.FirstOrDefault(m => m.Name == methodName);
            var handlerDelegateType = typeof(UnaryServerMethod<,>).MakeGenericType(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);

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