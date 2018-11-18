using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extensions;
using Grpc.Extensions.Consul.ServerSide;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Server.Interceptors;
using Sample.Grpc.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Grpc.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcServer(this IServiceCollection services)
        {
            services
                .AddGrpcService<Service1Impl>()
                .AddGrpcService<Service2Impl>()
                .AddSingleton<Interceptor, AccessInterceptor>()
                //.AddHostedService<Hosted>()
                .UseHostedGrpcServer(options =>
                {
                 
                })
                .UseConsulServiceRegister()
                .UseTcpCheck()
               
               
                ;


            return services;
        }
    }
}
