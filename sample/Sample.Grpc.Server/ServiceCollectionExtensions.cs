using Grpc.Extensions;
using Grpc.Extensions.ServerSide.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Server.Handlers;
using Sample.Grpc.Server.Interceptors;
using Sample.Grpc.Server.Services;

namespace Sample.Grpc.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcServer(this IServiceCollection services)
        {
            services
                .AddGrpcService<Service1Impl>()
                .AddGrpcService<Service2Impl>()
                .AddScoped<Handler1>()
                .AddScoped<Handler2>()
                .AddScoped<A>()
                .AddSingleton<ServerInterceptor, AccessInterceptor>()
                .AddHostedGrpcServer(options =>
                {
                })
                //.UseConsulServiceRegister()
                //.UseTcpCheck()
                //.AddSingleton<IServiceDefinitionProvider, MyServiceDefinitionProvider>()
                ;

            return services;
        }
    }
}