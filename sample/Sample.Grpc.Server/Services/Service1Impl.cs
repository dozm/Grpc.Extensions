using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Messages;
using Sample.Services;
using static Sample.Services.Service1;

namespace Sample.Grpc.Server.Services
{
    public class Service1Impl : Service1Base, IGrpcSerivce
    {
        private readonly ILogger<Service1Impl> _logger;

        public Service1Impl(ILogger<Service1Impl> logger)
        {
            _logger = logger;
        }
        public ServerServiceDefinition BuildServiceDefinition()
        {

            return Service1.BindService(this);
        }

        public override async Task<Response1> API1(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API1 收到请求：{request.Message}");
            return new Response1 { Message = $"Service1Impl API1 {DateTime.Now}" };
        }

        public override async Task<Response1> API2(Request1 request, ServerCallContext context)
        {
            _logger.LogInformation($"API2 收到请求：{request.Message}");
            return new Response1 { Message = $"Service1Impl API2 {DateTime.Now}" };
        }


    }
}
