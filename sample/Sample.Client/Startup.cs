using Grpc.Extensions.ClientSide;
using Grpc.Extensions.ClientSide.Interceptors;
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
            .AddSingleton<ClientInterceptor, AccessInterceptor>()
            .AddGrpcClient<Service1Client>()
            .AddGrpcClient<Service2Client>()
            .AddGrpcClientExtensions(options =>
            {
                //options.ServiceMaps.Add<Service1Client>("192.168.4.128", 9001);
                //options.ServiceMaps.Add<Service2Client>("192.168.4.128", 9001);

                options.ServiceMaps.Add<Service1Client>("172.20.10.12", 9001);
                options.ServiceMaps.Add<Service2Client>("172.20.10.12", 9001);
            })

            //.UseConsulServiceDiscovery(options =>
            //{
            //    //options.Address = "http://192.168.8.6:8500";
            //    options.Address = "http://172.20.10.3:8500";

            //    options.ServiceMaps.Add<Service1Client>("testsvc");
            //    options.ServiceMaps.Add<Service2Client>("testsvc");
            //})
            ;
        }
    }
}