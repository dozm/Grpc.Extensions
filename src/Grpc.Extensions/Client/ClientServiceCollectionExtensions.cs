using Grpc.Core;
using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection UseGrpcClientExtensions(this IServiceCollection services)
        {
            services.TryAddTransient<IExecutionStrategyFactory, ExecutionStrategyFactory>();
            services.TryAddSingleton<StatelessCallInvoker>();
            services.TryAddSingleton<CallInvoker, CallInvokerWrapper>();
            services.TryAddSingleton<IInterceptorProvider, InterceptorProvider>();
            services.TryAddSingleton<IChannelProvider, ChannelProvider>();
            services.ConfigureOptions<GrpcClientOptionsConfigurator>();


            return services;
        }

        public static IServiceCollection AddGrpcClient<TClient>(this IServiceCollection services)
            where TClient : class
        {
            return services.AddGrpcClient(typeof(TClient));
        }

        public static IServiceCollection AddGrpcClient(this IServiceCollection services, Type clientType)
        {
            services.AddTransient(clientType);

            services.Configure<GrpcClientOptions>(options =>
            {
                options.Clients.Add(new ClientMetadata(clientType));
            });

            return services;
        }
    }
}
