using Grpc.Extensions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseHostedGrpcServer(this IServiceCollection services)
        {
            services.TryAddTransient<IServiceDefinitionProvider, ServiceDefinitionProvider>();
            services.TryAddTransient<IInterceptorProvider, InterceptorProvider>();
            services.ConfigureOptions<GrpcServerOptionsConfigurator>();
            services.AddHostedService<HostedGrpcServer>();            

            return services;
        }

        public static IServiceCollection UseGrpcServer(this IServiceCollection services, Action<GrpcServerOptions> configureOptions)
        {
            services.UseHostedGrpcServer();
            services.Configure(configureOptions);
            
            return services;
        }
    }
}
