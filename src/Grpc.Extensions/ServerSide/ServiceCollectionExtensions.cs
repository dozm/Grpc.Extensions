﻿using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.Internal;
using Grpc.Extensions.ServerSide;
using Grpc.Extensions.ServerSide.HealthCheck;
using Grpc.Extensions.ServerSide.Interceptors;
using Grpc.Extensions.ServerSide.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using static Grpc.Health.V1.Health;

namespace Grpc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHostedGrpcServer(this IServiceCollection services)
        {
            services.TryAddTransient<IExecutionStrategyFactory, ExecutionStrategyFactory>();
            services.TryAddTransient<IServiceDefinitionProvider, ServiceDefinitionProvider>();
            services.ConfigureOptions<GrpcServerOptionsConfigurator>();
            services.AddHostedService<HostedGrpcServer>();
            services.TryAddSingleton<IGrpcServerContextAccessor, GrpcServerContextAccessor>();
            services.TryAddSingleton<IGrpcContextAccessor, GrpcContextAccessor>();

            services.TryAddSingleton<IServerInterceptorProvider, ServerInterceptorProvider>();
            services.AddSingleton<ServerInterceptor, CreateContextInterceptor>();
            services.AddSingleton<ServerInterceptor, ServerExceptionHandleInterceptor>();

            services.TryAddSingleton<Router>();
            services.TryAddSingleton<IRoutingTable, RoutingTable>();

            return services;
        }

        public static IServiceCollection AddHostedGrpcServer(this IServiceCollection services, Action<GrpcServerOptions> configureOptions)
        {
            services.AddHostedGrpcServer();
            services.Configure(configureOptions);

            return services;
        }

        public static IServiceCollection AddGrpcService<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddScoped<TService>();
            //services.AddTransient<IServiceDefinitionFactory, ServiceDefinitionFactory<TService>>();
            services.AddTransient<IServiceDefinitionFactory, ServiceDefinitionFactoryWithRouting<TService>>();
            services.Configure<GrpcServerOptions>(options =>
            {
                options.ServiceMetadatas.Add(new ServiceMetadata(typeof(TService)));
            });

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