using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
