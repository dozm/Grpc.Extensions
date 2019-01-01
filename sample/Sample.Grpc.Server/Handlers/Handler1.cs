using Grpc.Core;
using Sample.Messages;
using System;
using System.Threading.Tasks;

namespace Sample.Grpc.Server.Handlers
{
    public class Handler1
    {
        public async Task<Response1> API1(Request1 request, ServerCallContext context)
        {
            return new Response1 { Message = $"Handler1 API1 {DateTime.Now}" };
        }

        public async Task<Response1> API2(Request1 request, ServerCallContext context)
        {
            return new Response1 { Message = $"Handler1 API2 {DateTime.Now}" };
        }
    }
}