using Grpc.Core;
using Microsoft.Extensions.Logging;
using Sample.Messages;
using System;
using System.Threading.Tasks;
using static Sample.Services.Service2;

namespace Sample.Grpc.Server.Services
{
    public class Service2Impl : Service2Base
    {
        private readonly ILogger<Service1Impl> _logger;

        public Service2Impl(ILogger<Service1Impl> logger)
        {
            _logger = logger;
        }

        public override async Task<Response1> API1(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API1 收到请求：{request.Message}");
            return new Response1 { Message = $"Service2Impl API1 {DateTime.Now}" };
        }

        public override async Task<Response1> API2(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API2 收到请求：{request.Message}");
            return new Response1 { Message = $"Service2Impl API2 {DateTime.Now}" };
        }
    }
}