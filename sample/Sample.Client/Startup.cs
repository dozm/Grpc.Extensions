using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Sample.Grpc.Server;
using Microsoft.Extensions.Logging;
using Grpc.Core.Interceptors;
using static Sample.Services.Service1;
using static Sample.Services.Service2;
using Grpc.Extensions.Client;
using Sample.Client.Interceptors;

namespace Sample.Host
{
    public class Startup
    {
        public HostBuilderContext HostContext { get; }

        public Startup(HostBuilderContext hostContext)
        {
            HostContext = hostContext;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHostedService<Hosted>()
            .AddSingleton<Interceptor, AccessInterceptor>()
            .AddGrpcClient<Service1Client>()
            .AddGrpcClient<Service2Client>()
            .UseGrpcClientExtensions()
            ;

        }
    }
}
