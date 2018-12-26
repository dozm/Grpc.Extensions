using Grpc.Core;
using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.TryAddSingleton<IChannelFactory, ChannelFactory>();
            services.ConfigureOptions<GrpcClientOptionsConfigurator>();

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