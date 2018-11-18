using System;
using System.Collections.Generic;
using System.Text;
using Consul;

namespace Grpc.Extensions.Consul.ServerSide
{
    class TtlCheckRegistrationProvider : IAgentCheckRegistrationProvider
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
