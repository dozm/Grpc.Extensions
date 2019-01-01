using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Grpc.Extensions.ServerSide
{
    public class GrpcContext
    {
        internal IServiceScope ServiceScope { get; set; }
        public ServerCallContext ServerCallContext { get; set; }

        public IServiceProvider ServiceProvider => ServiceScope?.ServiceProvider;
    }
}