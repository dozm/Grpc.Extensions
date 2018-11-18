using System;
using System.Collections.Generic;
using System.Text;
using Consul;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class GrpcCheckRegistrationProvider : IAgentCheckRegistrationProvider
    {
        public AgentCheckRegistration[] GetCheckRegistration(AgentServiceRegistration serviceRegistration)
        {
            return new AgentCheckRegistration[]
             {
                new AgentGrpcCheckRegistration
                {
                    Name = $"gRPC {serviceRegistration.ID}",
                    GRPC= $"{serviceRegistration.Address}:{serviceRegistration.Port}",
                    GrpcUseTLS = false,
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(2),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60)
                },
             };
        }
    }
}
