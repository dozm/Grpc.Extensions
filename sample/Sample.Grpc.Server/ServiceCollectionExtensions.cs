﻿using Grpc.Extensions;
using Grpc.Extensions.Consul.ServerSide;
using Grpc.Extensions.ServerSide.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Sample.Grpc.Server.Interceptors;
using Sample.Grpc.Server.Services;

namespace Sample.Grpc.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSampleGrpcServer(this IServiceCollection services)
        {
            services
                .AddGrpcService<Service1Impl>()
                .AddGrpcService<Service2Impl>()
                .AddSingleton<ServerInterceptor, AccessInterceptor>()
                .AddHostedGrpcServer(options =>
                {
                    options.AddPort("172.20.10.12", 9001);
                    options.AddPort("[fe80::995f:ece5:2370:f4]", 9003);
                    //options.AddPort("192.168.4.128", 9001);

                })

                .UseConsulServiceRegister(options =>
                {
                    options.Address = "http://172.20.10.3:8500";
                    //options.Address = "http://192.168.8.6:8500";

                    options.ServiceRegistration.ConsulServiceName = "testsvc";
                })
                .UseTcpCheck()
                //.UseTtlCheck()
                //.UseGrpcCheck()
                //.UseHealthService()
                ;

            return services;
        }
    }
}