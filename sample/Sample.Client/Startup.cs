using Grpc.Core.Interceptors;
using Grpc.Extensions.ClientSide;
using Grpc.Extensions.Consul.ClientSide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Client.Interceptors;
using Sample.Grpc.Server;
using System.Collections.Generic;
using static Sample.Services.Service1;
using static Sample.Services.Service2;

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
            .Configure<GrpcClientOptions>(options =>
            {
                options.ServicesEndpoint[typeof(Service1Client)] = new List<ServiceEndPoint>()
                {
                    new ServiceEndPoint{ Address="192.168.4.128", Port=9001}
                };
                options.ServicesEndpoint[typeof(Service2Client)] = new List<ServiceEndPoint>()
                {
                    new ServiceEndPoint{ Address="192.168.4.128", Port=9001}
                };
            })

            .UseConsulServiceDiscovery()
            .Configure<ConsulClientOptions>(options =>
            {
                options.ServiceMap[typeof(Service1Client)] = "testsvc";
                options.ServiceMap[typeof(Service2Client)] = "testsvc";
            })
            ;
        }
    }
}