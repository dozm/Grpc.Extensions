using Grpc.Extensions.Consul.Options;

namespace Grpc.Extensions.Consul
{
    public class ConsulOptions
    {
        public string Address { get; set; }

        public ServiceRegistrationOptions ServiceRegistration { get; set; }
    }
}