using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Sample.Client.Interceptors
{
    public class AccessInterceptor : Interceptor
    {
        private readonly ILogger _logger;

        public AccessInterceptor(ILogger<AccessInterceptor> logger)
        {
            _logger = logger;
        }

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