using Consul;
using Grpc.Extensions.ServerSide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ServerSide
{
    public interface IAgentServiceRegistrationFactory
    {
        AgentServiceRegistration Create();
    }
}
