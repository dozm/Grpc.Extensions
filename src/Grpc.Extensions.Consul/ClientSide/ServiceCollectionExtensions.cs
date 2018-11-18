﻿using Grpc.Extensions.Client;
using Grpc.Extensions.Consul.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ClientSide
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseConsulServiceDiscovery(this IServiceCollection services)
        {
            services.ConfigureOptions<ConsulClientOptionsConfigurator>();
            services.AddTransient<IServiceDiscovery, ConsulServiceDiscovery>();
            services.AddTransient<IChannelProvider, ChannelProviderWithConsul>();

            return services;
        }
    }
}