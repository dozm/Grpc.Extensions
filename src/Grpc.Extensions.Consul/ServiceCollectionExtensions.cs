using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extensions.Consul
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseConsul(this IServiceCollection services)
        {
            services.AddTransient<IConsulClientFactory, ConsulClientFactory>();

            return services;
        }
    }
}