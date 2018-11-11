using System;
using Consul;
using Grpc.Extensions.ServerSide;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class AgentServiceRegistrationFactory : IAgentServiceRegistrationFactory
    {
        private readonly ConsulOptions _options;

        public AgentServiceRegistrationFactory(IOptions<ConsulOptions> options)
        {
            _options = options.Value;
        }

        public AgentServiceRegistration Create(GrpcServerContext context)
        {
            var defaultPort = context.GrpcServer.Ports.FirstOrDefault();
            if (defaultPort == null)
                throw new Exception($"未能创建默认的 Consul 服务注册，因为没有找到 Grpc Server 监听的端口。");

            var address = defaultPort.Host;
            var port = defaultPort.BoundPort;
            var serviceRegistrationOptions = _options.ServiceRegistration;

            var id = $"{serviceRegistrationOptions.ConsulServiceName}-{address}:{port}-{Guid.NewGuid()}";

            var serviceRegistration = new AgentServiceRegistration()
            {
                ID = id,
                Name = serviceRegistrationOptions.ConsulServiceName,
                Address = address,
                Port = port,
                EnableTagOverride = serviceRegistrationOptions.EnableTagOverride,
                Tags = new string[] { }
            };

            serviceRegistration.Check = CreateCheck(serviceRegistration);


            return serviceRegistration;
        }

        private AgentServiceCheck CreateCheck(AgentServiceRegistration serviceRegistration)
        {
            return new AgentCheckRegistration
            {
                ID = $"{serviceRegistration.ID}:ttlcheck",
                Name = "ttlcheck",
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                TTL = TimeSpan.FromSeconds(15),
                Status = HealthStatus.Passing,
                ServiceID = serviceRegistration.ID

            };
        }
    }
}
