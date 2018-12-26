using Consul;
using System;

namespace Grpc.Extensions.Consul.ServerSide
{
    internal class TtlCheckRegistrationProvider : IAgentCheckRegistrationProvider
    {
        public AgentCheckRegistration[] GetCheckRegistration(AgentServiceRegistration serviceRegistration)
        {
            return new AgentCheckRegistration[]
            {
                new AgentCheckRegistration
                {
                    Name = $"TTL {serviceRegistration.ID}",
                    ServiceID = serviceRegistration.ID,
                    TTL = TimeSpan.FromSeconds(15),
                    Status = HealthStatus.Critical,
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                }
            };
        }
    }
}