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
                .AddGrpcServer()

                ;
        }
    }
}