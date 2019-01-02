using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extensions.ClientSide.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Sample.Client.Interceptors
{
    public class ClientAccessInterceptor : ClientInterceptor
    {
        private readonly ILogger _logger;
        public override int Order => -101;

        public ClientAccessInterceptor(ILogger<ClientAccessInterceptor> logger)
        {
            _logger = logger;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"Client calling {context.Method.FullName} {context.Host}");

            var response = continuation(request, context);

            _logger.LogInformation($"Client called {context.Method.FullName} {context.Host}");
            return response;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var response = continuation(request, context);

            Func<Task<TResponse>> func = async () =>
            {
                try
                {
                    _logger.LogInformation($"Client calling {context.Method.FullName} {context.Host}");
                    return await response.ResponseAsync;
                }
                finally
                {
                    _logger.LogInformation($"Client called {context.Method.FullName} {context.Host}");
                }
            };

            return new AsyncUnaryCall<TResponse>(func(), response.ResponseHeadersAsync, response.GetStatus, response.GetTrailers, response.Dispose);
        }
    }
}