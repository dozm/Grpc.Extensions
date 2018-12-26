using Grpc.Core;
using Sample.Messages;
using System;
using System.Threading.Tasks;
using static Sample.Services.Service2;

namespace Sample.Grpc.Server.Services
{
    public class Service2Impl : Service2Base
    {
        public override async Task<Response1> API1(Request1 request, ServerCallContext context)
        {
            return new Response1 { Message = $"Service2Impl API1 {DateTime.Now}" };
        }

        public override async Task<Response1> API2(Request1 request, ServerCallContext context)
        {
            return new Response1 { Message = $"Service2Impl API2 {DateTime.Now}" };
        }
    }
}