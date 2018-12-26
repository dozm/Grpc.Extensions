using Consul;
using System;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class TcpCheckRegistrationProvider : IAgentCheckRegistrationProvider
    {
        public AgentCheckRegistration[] GetCheckRegistration(AgentServiceRegistration serviceRegistration)
        {
            return new AgentCheckRegistration[]
            {
                new AgentCheckRegistration
                {
                    Name = $"TCP {serviceRegistration.ID}",
                    TCP = $"{serviceRegistration.Address}:{serviceRegistration.Port}",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(2),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                },
            };
        }
    }
}