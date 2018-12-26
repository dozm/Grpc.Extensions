using Consul;

namespace Grpc.Extensions.Consul.ServerSide
{
    public interface IAgentCheckRegistrationProvider
    {
        AgentCheckRegistration[] GetCheckRegistration(AgentServiceRegistration serviceRegistration);
    }
}