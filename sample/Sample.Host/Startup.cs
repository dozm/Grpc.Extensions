using Grpc.Extensions;
using Grpc.Extensions.Consul.ServerSide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Grpc.Server;

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
            services
                .AddGrpcServices()
                .AddHostedGrpcServer(options =>
                {
                    options.AddPort("172.20.10.12", 9001);                    
                    options.AddPort("[fe80::995f:ece5:2370:f4]", 9003);
                    //options.AddPort("192.168.4.128", 9001);

                })
                //.UseConsulServiceRegister(options =>
                //{                   
                //    options.Address = "http://172.20.10.3:8500";
                //    //options.Address = "http://192.168.8.6:8500";

                //    options.ServiceRegistration.ConsulServiceName = "testsvc";
                //})
                //.UseTcpCheck()
                //.UseTtlCheck()
                //.UseGrpcCheck()
                //.UseHealthService()
                ;
        }
    }
}