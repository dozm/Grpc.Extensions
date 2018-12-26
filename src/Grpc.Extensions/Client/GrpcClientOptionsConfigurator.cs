using Grpc.Extensions.Client.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Grpc.Extensions.Client
{
    public class GrpcClientOptionsConfigurator : IConfigureOptions<GrpcClientOptions>
    {
        readonly private IConfiguration _config;

        public GrpcClientOptionsConfigurator(IConfiguration config)
        {
            _config = config;
        }

        public void Configure(GrpcClientOptions options)
        {
            var grpcClientConfig = _config.GetSection("GrpcClientOptions");
            if (grpcClientConfig == null)
                return;

            var services = BindServices(grpcClientConfig);

            services.ForEach(se => options.ServicesEndpoint.Add(se.ServiceName, se));
        }

        private List<ServiceEndpoint> BindServices(IConfiguration config)
        {
            var dic = config.GetSection("Services")?.Get<Dictionary<string, string>>();

            var services = new List<ServiceEndpoint>();
            if (dic != null)
            {
                foreach (var kvp in dic)
                {
                    services.Add(new ServiceEndpoint
                    {
                        ServiceName = kvp.Key,
                        Endpoints = kvp.Value.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    });
                }
            }

            return services;
        }
    }
}