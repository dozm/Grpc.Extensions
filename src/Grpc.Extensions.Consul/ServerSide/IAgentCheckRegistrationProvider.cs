using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ServerSide
{
    public interface IAgentCheckRegistrationProvider
    {
        AgentCheckRegistration[] GetCheckRegistration(AgentServiceRegistration serviceRegistration);
    }
}
