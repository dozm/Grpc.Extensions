using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Extensions.ServerSide.Interceptors
{
    internal class ServerExceptionHandleInterceptor : ServerInterceptor
    {
        private readonly ILogger<ServerExceptionHandleInterceptor> _logger;

        public override int Order { get; set; } = -100;

        public ServerExceptionHandleInterceptor(ILogger<ServerExceptionHandleInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            Exception exception = null;
            try
            {
                return await continuation(request, context);
            }
            catch (RpcException ex)
            {
                exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw new RpcException(new Status(StatusCode.Unknown, GetExceptionDetail(ex)));
            }
            finally
            {
                if (exception != null)
                    _logger.LogError(exception, "");
            }
        }

        private string GetExceptionDetail(Exception ex, int depth = 0)
        {
            const int maxDepth = 5;
            var sb = new StringBuilder();
            sb.AppendLine($"{ex.GetType().FullName}: {ex.Message}")
                .AppendLine(ex.StackTrace);

            if (++depth > maxDepth)
            {
                sb.Append("...");
            }
            else if (ex.InnerException != null)
            {
                sb.AppendLine($"InnerException: {GetExceptionDetail(ex.InnerException, depth)}");
            }

            return sb.ToString();
        }
    }
}