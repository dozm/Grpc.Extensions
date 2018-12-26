using Consul;
using Microsoft.Extensions.Options;
using System;

namespace Grpc.Extensions.Consul
{
    public class ConsulClientFactory : IConsulClientFactory
    {
        private readonly ConsulOptions _options;

        public ConsulClientFactory(IOptions<ConsulOptions> options)
        {
            _options = options.Value;
        }

        public ConsulClient Create()
        {
            if (string.IsNullOrEmpty(_options.Address))
            {
                throw new ArgumentException("未配置 Consul 服务器地址。");
            }

            return new ConsulClient(config =>
            {
                config.Address = new Uri(_options.Address);
            });
        }
    }
}