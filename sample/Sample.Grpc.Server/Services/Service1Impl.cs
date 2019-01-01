using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sample.Messages;
using System;
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
                await Task.Delay(5000);
                //var x = 0;
                //var a = 1 / x;

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
    }
}