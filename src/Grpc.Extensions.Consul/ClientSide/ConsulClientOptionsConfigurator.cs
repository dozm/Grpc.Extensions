using Grpc.Extensions.ClientSide;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ConsulClientOptionsConfigurator : IConfigureOptions<ConsulClientOptions>
    {
        private readonly IConfiguration _config;
        private readonly GrpcClientOptions _grpcClientOptions;

        public ConsulClientOptionsConfigurator(IConfiguration config, IOptions<GrpcClientOptions> grpcClientOptions)
        {
            _config = config;
            _grpcClientOptions = grpcClientOptions.Value;
        }

        public void Configure(ConsulClientOptions options)
        {
            var config = _config.GetSection("Consul");
            if (config == null)
                return;

            var address = config.GetSection("Address")?.Get<string>();
            if (address != null)
                options.Address = address;

            ConfigureServiceMap(options);
        }

        private void ConfigureServiceMap(ConsulClientOptions options)
        {
            var config = _config.GetSection("Consul:ServiceMap");
            if (config == null)
                return;

            var dic = config.Get<Dictionary<string, string[]>>();
        }
    }
}