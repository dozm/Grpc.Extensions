using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grpc.Extensions.Test.Interceptors
{
    public class ServerLogInterceptor : Interceptor
    {
        private readonly ILogger _logger;

        public ServerLogInterceptor(ILogger<ServerLogInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            Console.WriteLine("log start");
            _logger.LogInformation("log start");

            var response = await continuation(request, context);

            _logger.LogInformation("log end");
            Console.WriteLine("log end");

            return response;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}