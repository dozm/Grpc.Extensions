using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.Internal;
using Grpc.Extensions.ServerSide;
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
            services.TryAddTransient<IExecutionStrategyFactory, ExecutionStrategyFactory>();
            services.TryAddTransient<IServiceDefinitionProvider, ServiceDefinitionProvider>();
            services.TryAddTransient<IInterceptorProvider, InterceptorProvider>();
            services.ConfigureOptions<GrpcServerOptionsConfigurator>();
            services.AddHostedService<HostedGrpcServer>();
            services.AddSingleton<IGrpcServerContextAccessor, GrpcServerContextAccessor>();


            return services;
        }

        public static IServiceCollection UseGrpcServer(this IServiceCollection services, Action<GrpcServerOptions> configureOptions)
        {
            services.UseHostedGrpcServer();
            services.Configure(configureOptions);

            return services;
        }

        public static IServiceCollection AddGrpcService<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddTransient<TService>();
            services.AddTransient<IServiceDefinitionFactory, ServiceDefinitionFactory<TService>>();

            return services;
        }
    }
}
