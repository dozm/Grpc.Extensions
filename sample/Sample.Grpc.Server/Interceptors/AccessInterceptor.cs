using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sample.Grpc.Server.Interceptors
{
    public class AccessInterceptor : Interceptor
    {
        private readonly ILogger _logger;

        public AccessInterceptor(ILogger<AccessInterceptor> logger)
        {
            _logger = logger;
        }

        #region 服务器端拦截方法

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"UnaryServerHandler log start {context.Host}  {context.Method}");

            var response = await continuation(request, context);

            _logger.LogInformation($"UnaryServerHandler log end {context.Host}  {context.Method}");

            return response;
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"ClientStreamingServerHandler log start {context.Host}  {context.Method}");

            var response = await continuation(requestStream, context);

            _logger.LogInformation($"ClientStreamingServerHandler log end {context.Host}  {context.Method}");
            return response;
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"DuplexStreamingServerHandler log start {context.Host}  {context.Method}");

            await continuation(requestStream, responseStream, context);

            _logger.LogInformation($"DuplexStreamingServerHandler log end {context.Host}  {context.Method}");
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"ServerStreamingServerHandler log start {context.Host}  {context.Method}");

            await continuation(request, responseStream, context);

            _logger.LogInformation($"ServerStreamingServerHandler log end {context.Host}  {context.Method}");
        }

        #endregion 服务器端拦截方法

        #region 客户端拦截方法

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"BlockingUnaryCall log start {context.Host}  {context.Method.Name}");

            var response = continuation(request, context);

            _logger.LogInformation($"BlockingUnaryCall log end {context.Host}  {context.Method.Name}");
            return response;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"AsyncUnaryCall log start {context.Host}  {context.Method.Name}");
            var response = continuation(request, context);

            _logger.LogInformation($"AsyncUnaryCall log end {context.Host}  {context.Method.Name}");
            return response;
        }

        #endregion 客户端拦截方法
    }
}