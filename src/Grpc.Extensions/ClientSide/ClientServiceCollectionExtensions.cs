using Grpc.Core;
using Grpc.Extensions.ClientSide.Interceptors;
using Grpc.Extensions.ExecutionStrategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Grpc.Extensions.ClientSide
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcClientExtensions(this IServiceCollection services)
        {
            services.TryAddTransient<IExecutionStrategyFactory, ExecutionStrategyFactory>();
            services.TryAddSingleton<StatelessCallInvoker>();
            services.TryAddSingleton<CallInvoker, InterceptingCallInvoker>();
            services.TryAddSingleton<IChannelProvider, ChannelProvider>();
            services.TryAddSingleton<IChannelFactory, ChannelFactory>();
            services.TryAddSingleton<IServiceDiscovery, DefaultServiceDiscovery>();
            services.TryAddSingleton<ILoadBalancerProvider, LoadBalancerProvider>();
            services.ConfigureOptions<GrpcClientOptionsConfigurator>();

            services.TryAddSingleton<IClientInterceptorProvider, ClientInterceptorProvider>();
            services.AddSingleton<ClientInterceptor, ClientExceptionHandleInterceptor>();

            return services;
        }

        public static IServiceCollection AddGrpcClientExtensions(this IServiceCollection services, Action<GrpcClientOptions> action)
        {
            services.AddGrpcClientExtensions();
            services.Configure(action);

            return services;
        }

        public static IServiceCollection AddGrpcClient<TClient>(this IServiceCollection services)
            where TClient : ClientBase
        {
            services.AddTransient<TClient>();

            services.Configure<GrpcClientOptions>(options =>
            {
                options.Clients.Add(new ClientMetadata(typeof(TClient)));
            });

            return services;
        }

        //public static IServiceCollection AddGrpcClient(this IServiceCollection services, Type clientType)
        //{
        //    services.AddTransient()

        //    services.AddTransient(clientType);

        //    services.Configure<GrpcClientOptions>(options =>
        //    {
        //        options.Clients.Add(new ClientMetadata(clientType));
        //    });

        //    return services;
        //}
    }
}