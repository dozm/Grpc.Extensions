using Grpc.Extensions.Consul.Options;
using Grpc.Extensions.ServerSide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Grpc.Extensions.Consul.ServerSide
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseConsulServiceRegister(this IServiceCollection services)
        {
            services.ConfigureOptions<ConsulOptionsConfigurator>();

            services.AddHostedService<ServiceRegistrar>();
            services.TryAddTransient<IConsulClientFactory, ConsulClientFactory>();
            services.AddTransient<IAgentServiceRegistrationFactory, AgentServiceRegistrationFactory>();
          

            return services;
        }

        public static IServiceCollection UseGrpcCheck(this IServiceCollection services)
        {
            services.UseHealthService();
            services.AddTransient<IAgentCheckRegistrationProvider, GrpcCheckRegistrationProvider>();
            return services;
        }

        public static IServiceCollection UseTtlCheck(this IServiceCollection services)
        {
            services.AddTransient<IAgentCheckRegistrationProvider, TtlCheckRegistrationProvider>();
            return services;
        }

        public static IServiceCollection UseTcpCheck(this IServiceCollection services)
        {
            services.AddTransient<IAgentCheckRegistrationProvider, TcpCheckRegistrationProvider>();
            return services;
        }
    }
}
