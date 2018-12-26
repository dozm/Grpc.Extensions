using Grpc.Core.Interceptors;
using Grpc.Extensions.Client;
using Grpc.Extensions.Consul.ClientSide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Client.Interceptors;
using Sample.Grpc.Server;
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