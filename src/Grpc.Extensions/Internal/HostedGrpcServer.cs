using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Internal
{
    internal class HostedGrpcServer : IHostedService
    {
        private readonly IServiceDefinitionProvider _serviceDefinitionProvider;
        private readonly IInterceptorProvider _interceptorProvider;
        private readonly GrpcServerOptions _options;
        private readonly ILogger<HostedGrpcServer> _logger;
        private Server _server;
        public HostedGrpcServer(
            IServiceDefinitionProvider serviceDefinitionProviders,
            IInterceptorProvider interceptorProvider,
            IOptions<GrpcServerOptions> options,
            ILogger<HostedGrpcServer> logger)
        {
            _serviceDefinitionProvider = serviceDefinitionProviders;
            _interceptorProvider = interceptorProvider;
            _options = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _server = BuildServer();

            _logger.LogInformation("Grpc server starting");
            _server.Start();
            _logger.LogInformation("Grpc server started");

            if (_server.Ports.Count() > 0)
            {
                var listening = _server.Ports.Select(p => $"{p.Host}:{p.BoundPort}").Aggregate((ports, port) => $"{ports}; {port}");
                _logger.LogInformation($"Listening: {listening}");
            }

            return Task.CompletedTask;
        }

        private Server BuildServer()
        {
            var builder = new ServerBuilder();
            builder.AddServiceDefinition(_serviceDefinitionProvider.GetServiceDefinitions())
               .UseInterceptor(_interceptorProvider.GetInterceptors());

            if (_options == null)
                return builder.Build();

            _options.ServerPorts?.ForEach(p => builder.AddPort(p));
            _options.ChannelOptions?.ForEach(o => builder.AddChannelOption(o));

            return builder.Build();
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Grpc server stopping");

            if (_server != null)
            {

                await _server.ShutdownAsync();
            }

            _logger.LogInformation("Grpc server stopped");
        }
    }
}
