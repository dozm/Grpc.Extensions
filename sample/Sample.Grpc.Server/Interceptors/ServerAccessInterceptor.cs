﻿using Grpc.Core;
using Grpc.Extensions.ServerSide.Interceptors;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Sample.Grpc.Server.Interceptors
{
    public class ServerAccessInterceptor : ServerInterceptor
    {
        private readonly ILogger _logger;

        public override int Order => -101;

        public ServerAccessInterceptor(ILogger<ServerAccessInterceptor> logger)
        {
            _logger = logger;
        }

        #region 服务器端拦截方法

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"=================================================");
            _logger.LogInformation($"Server handling call {context.Method} {context.Host}");

            try
            {
                return await continuation(request, context);
            }
            finally
            {
                _logger.LogInformation($"Server handled call {context.Method} {context.Host}");
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"=================================================");
            _logger.LogInformation($"Server handling call {context.Method} {context.Host}");

            try
            {
                return await continuation(requestStream, context);
            }
            finally
            {
                _logger.LogInformation($"Server handled call {context.Method} {context.Host}");
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"=================================================");
            _logger.LogInformation($"Server handling call {context.Method} {context.Host}");

            try
            {
                await continuation(requestStream, responseStream, context);
            }
            finally
            {
                _logger.LogInformation($"Server handled call {context.Method} {context.Host}");
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation($"=================================================");
            _logger.LogInformation($"Server handling call {context.Method} {context.Host}");
            try
            {
                await continuation(request, responseStream, context);
            }
            finally
            {
                _logger.LogInformation($"Server handled call {context.Method} {context.Host}");
            }
        }

        #endregion 服务器端拦截方法
    }
}