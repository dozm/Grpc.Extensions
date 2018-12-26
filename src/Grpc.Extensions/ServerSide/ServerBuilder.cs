using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Collections.Generic;
using System.Linq;

namespace Grpc.Extensions
{
    public class ServerBuilder
    {
        private readonly List<Interceptor> _interceptors = new List<Interceptor>();
        private readonly List<ServerPort> _serverPorts = new List<ServerPort>();
        private readonly List<ChannelOption> _channelOptions = new List<ChannelOption>();

        private readonly List<ServerServiceDefinition> _serviceDefinitions = new List<ServerServiceDefinition>();

        public ServerBuilder UseInterceptor(Interceptor interceptor)
        {
            _interceptors.Add(interceptor);
            return this;
        }

        public ServerBuilder AddPort(ServerPort serverPort)
        {
            _serverPorts.Add(serverPort);
            return this;
        }

        public ServerBuilder AddServiceDefinition(ServerServiceDefinition serviceDefinition)
        {
            _serviceDefinitions.Add(serviceDefinition);
            return this;
        }

        public ServerBuilder AddChannelOption(ChannelOption option)
        {
            _channelOptions.Add(option);

            return this;
        }

        public Server Build()
        {
            Server server = _channelOptions != null && _channelOptions.Count > 0 ? new Server(_channelOptions) : new Server();

            var serviceDefinitions = ApplyInterceptor(_serviceDefinitions, _interceptors.ToArray());

            foreach (var service in serviceDefinitions)
            {
                server.Services.Add(service);
            }

            _serverPorts.ForEach(port => server.Ports.Add(port));

            return server;
        }

        private static IEnumerable<ServerServiceDefinition> ApplyInterceptor(IEnumerable<ServerServiceDefinition> serviceDefinitions, Interceptor[] interceptors)
        {
            return serviceDefinitions.Select(serviceDefinition => serviceDefinition.Intercept(interceptors));
        }
    }
}