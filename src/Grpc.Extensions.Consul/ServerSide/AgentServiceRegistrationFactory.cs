using Consul;
using Grpc.Extensions.ServerSide;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class AgentServiceRegistrationFactory : IAgentServiceRegistrationFactory
    {
        private readonly ConsulOptions _options;
        private readonly IGrpcServerContextAccessor _grpcServerContextAccessor;
        private readonly IAgentCheckRegistrationProvider _checkRegistrationProvider;

        public AgentServiceRegistrationFactory(IOptions<ConsulOptions> options, IGrpcServerContextAccessor grpcServerContextAccessor,
            IAgentCheckRegistrationProvider checkRegistrationProvider)
        {
            _options = options.Value;
            _grpcServerContextAccessor = grpcServerContextAccessor;
            _checkRegistrationProvider = checkRegistrationProvider;
        }

        public AgentServiceRegistration Create()
        {
            var serviceRegistrationOptions = _options.ServiceRegistration;

            var (host, port) = GetValidServiceEndpoint();

            var serviceRegistration = new AgentServiceRegistration()
            {
                ID = CreateServiceId(serviceRegistrationOptions.ConsulServiceName, host, port),
                Name = serviceRegistrationOptions.ConsulServiceName,
                Address = host,
                Port = port,
                EnableTagOverride = serviceRegistrationOptions.EnableTagOverride,
                Tags = serviceRegistrationOptions.Tags
            };

            serviceRegistration.Checks = _checkRegistrationProvider.GetCheckRegistration(serviceRegistration);

            return serviceRegistration;
        }

        private (string host, int port) GetValidServiceEndpoint()
        {
            string host = _options.ServiceRegistration?.ServiceHost;
            int port = _options.ServiceRegistration?.ServicePort ?? 0;

            var endpoints = _grpcServerContextAccessor.Context.GrpcServer.Ports;

            if (string.IsNullOrEmpty(host))
            {
                host = endpoints.FirstOrDefault()?.Host;
            }

            if (host == null)
            {
                throw new Exception("没有找到可用于注册到 Consul 的服务IP");
            }

            if (port == 0)
            {
                port = endpoints.FirstOrDefault(ed => ed.Host == host)?.BoundPort ?? (endpoints.FirstOrDefault()?.BoundPort ?? 0);
            }

            return (ParseIP(host), port);
        }

        private string CreateServiceId(string serviceName, string host, int port)
        {
            var endpoint = "";
            if (IPAddress.TryParse(host, out var ip))
                endpoint = new IPEndPoint(ip, port).ToString();
            else
                endpoint = $"{host}:{port}";

            return $"{serviceName}-{endpoint}-{Guid.NewGuid().ToString("N")}";
        }

        private string ParseIP(string host)
        {
            var address = host.Trim(new char[] { '[', ']' });

            if (!IPAddress.TryParse(address, out var ip))
            {
                return host;
            }

            if (ip.Equals(IPAddress.IPv6Any))
            {
                return "0:0:0:0:0:0:0:0";
            }

            if (ip.Equals(IPAddress.IPv6Loopback))
            {
                return "0:0:0:0:0:0:0:1";
            }

            return ip.ToString();
        }
    }
}