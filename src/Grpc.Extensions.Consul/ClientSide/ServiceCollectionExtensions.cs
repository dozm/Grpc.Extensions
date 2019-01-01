using Grpc.Extensions.ClientSide;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Grpc.Extensions.Consul.ClientSide
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseConsulServiceDiscovery(this IServiceCollection services)
        {
            services.ConfigureOptions<ConsulClientOptionsConfigurator>();
            services.AddTransient<IServiceDiscovery, ConsulServiceDiscovery>();
            services.AddSingleton<ILoadBalancerProvider, LoadBalancerProvider>();
            return services;
        }

        public static IServiceCollection UseConsulServiceDiscovery(this IServiceCollection services, Action<ConsulClientOptions> action)
        {
            services.UseConsulServiceDiscovery()
                .Configure(action);

            return services;
        }
    }
}