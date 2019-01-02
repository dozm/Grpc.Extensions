using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sample.Messages;
using System;
using System.Text;
using System.Threading.Tasks;
using static Sample.Services.Service1;

namespace Sample.Grpc.Server.Services
{
    public class Service1Impl : Service1Base
    {
        private readonly ILogger<Service1Impl> _logger;

        public Service1Impl(ILogger<Service1Impl> logger)
        {
            _logger = logger;
        }

        public override async Task<Response1> API1(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API1 收到请求：{request.Message}");

            try
            {

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"调用服务时发生异常", ex);
            }

            return new Response1 { Message = $"Service1Impl API1 {DateTime.Now}  handler hash {this.GetHashCode()}" };
        }

        public override async Task<Response1> API2(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API2 收到请求：{request.Message}");
            return new Response1 { Message = $"Service1Impl API2 {DateTime.Now}  handler hash {this.GetHashCode()}" };
        }

        public override async Task<Response1> ClientStreamAPI(IAsyncStreamReader<Request1> requestStream, ServerCallContext context)
        {
            int count = 0;
            var sb = new StringBuilder();
            while (await requestStream.MoveNext(default))
            {
                count++;
                var playload = requestStream.Current.Message;
                sb.Append(playload);
                _logger.LogInformation($"接收客户端流 NO.{count}: {playload}");
            }
            _logger.LogInformation($"{sb},读取次数：{count}");
            return new Response1 { Message = $"服务器以读: {sb}" };
        }

        public override async Task<Response1> ServerStreamAPI(IAsyncStreamReader<Request1> requestStream, ServerCallContext context)
        {
            return await base.ServerStreamAPI(requestStream, context);
        }

        public override async Task DuplexStreamingAPI(IAsyncStreamReader<Request1> requestStream, IServerStreamWriter<Response1> responseStream, ServerCallContext context)
        {
            await base.DuplexStreamingAPI(requestStream, responseStream, context);
        }

    }
}