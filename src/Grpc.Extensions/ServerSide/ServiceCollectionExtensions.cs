using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.Internal;
using Grpc.Extensions.ServerSide;
using Grpc.Extensions.ServerSide.HealthCheck;
using Grpc.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static Grpc.Health.V1.Health;

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

        public static IServiceCollection UseHostedGrpcServer(this IServiceCollection services, Action<GrpcServerOptions> configureOptions)
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

        public static IServiceCollection UseHealthService(this IServiceCollection services)
        {
            services.AddTransient<HealthBase, HealthService>();

            if (services.FirstOrDefault(s =>
                s.ImplementationType == typeof(ServiceDefinitionFactory<HealthBase>)
             && s.ServiceType == typeof(IServiceDefinitionFactory)) == null)
            {
                services.AddTransient<IServiceDefinitionFactory, ServiceDefinitionFactory<HealthBase>>();
            }

            return services;
        }

        public static IServiceCollection UseHealthService<THealth>(this IServiceCollection services)
            where THealth : HealthBase
        {
            services.AddTransient<HealthBase, THealth>();

            // 预防 ServiceDefinitionFactory<HealthBase> 被注册多次，导致创建 HealthBase 的多个服务定义。
            if (services.FirstOrDefault(s =>
                s.ImplementationType == typeof(ServiceDefinitionFactory<HealthBase>)
             && s.ServiceType == typeof(IServiceDefinitionFactory)) == null)
            {
                services.AddTransient<IServiceDefinitionFactory, ServiceDefinitionFactory<HealthBase>>();
            }

            return services;
        }
    }
}
