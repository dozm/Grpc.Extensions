using Consul;

namespace Grpc.Extensions.Consul.ServerSide
{
    public interface IAgentServiceRegistrationFactory
    {
        AgentServiceRegistration Create();
    }
}