using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    public class ClientExceptionHandleInterceptor : ClientInterceptor
    {
        private readonly ILogger<ClientExceptionHandleInterceptor> _logger;

        public override int Order => -100;

        public ClientExceptionHandleInterceptor(ILogger<ClientExceptionHandleInterceptor> logger)
        {
            _logger = logger;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            try
            {
                return continuation(request, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                throw ex;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var response = continuation(request, context);

            Func<Task<TResponse>> func = async () =>
            {
                try
                {
                    return await response.ResponseAsync;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                    throw ex;
                }
            };

            return new AsyncUnaryCall<TResponse>(func(), response.ResponseHeadersAsync, response.GetStatus, response.GetTrailers, response.Dispose);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var response = continuation(request, context);
            var responseStream = new LoggableAsyncStreamReader<TResponse>(response.ResponseStream, _logger);

            return new AsyncServerStreamingCall<TResponse>(responseStream, response.ResponseHeadersAsync, response.GetStatus, response.GetTrailers, response.Dispose);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var response = continuation(context);
            var requestStream = new LogableClientStreamWriter<TRequest>(response.RequestStream, _logger);

            return new AsyncClientStreamingCall<TRequest, TResponse>(requestStream, response.ResponseAsync, response.ResponseHeadersAsync, response.GetStatus, response.GetTrailers, response.Dispose);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return continuation(context);
        }
    }
}