using Grpc.Extensions;
using Grpc.Extensions.ServerSide.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Server.Interceptors;
using Sample.Grpc.Server.Services;

namespace Sample.Grpc.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcServices(this IServiceCollection services)
        {
            services
                .AddGrpcService<Service1Impl>()
                .AddGrpcService<Service2Impl>()
                .AddSingleton<ServerInterceptor, AccessInterceptor>()
                ;

            return services;
        }
    }
}