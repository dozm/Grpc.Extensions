using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extensions;
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
            services.AddTransient<IGrpcSerivce, Service1Impl>()
                .AddTransient<IGrpcSerivce, Service2Impl>()
                .AddSingleton<Interceptor, AccessInterceptor>()
                //.AddHostedService<Hosted>()
                .UseGrpcServer(options =>
                {
                    options.AddPort(9001);
                  
                })
                ;


            return services;
        }
    }
}
