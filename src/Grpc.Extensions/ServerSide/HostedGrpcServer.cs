using Grpc.Core;
using Grpc.Extensions.Logging;
using Grpc.Extensions.ServerSide;
using Grpc.Extensions.ServerSide.Interceptors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Internal
{
    internal class HostedGrpcServer : IHostedService
    {
        private readonly IServiceDefinitionProvider _serviceDefinitionProvider;
        private readonly IServerInterceptorProvider _interceptorProvider;
        private readonly GrpcServerOptions _options;
        private readonly ILogger<HostedGrpcServer> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEnumerable<IGrpcServerLifetime> _lifetimes;
        private readonly IGrpcServerContextAccessor _contextAccessor;
        private Server _server;

        public HostedGrpcServer(
            IServiceDefinitionProvider serviceDefinitionProviders,
            IServerInterceptorProvider interceptorProvider,
            IOptions<GrpcServerOptions> options,
            IEnumerable<IGrpcServerLifetime> lifetimes,
            IGrpcServerContextAccessor contextAccessor,
            ILoggerFactory loggerFactory)
        {
            _serviceDefinitionProvider = serviceDefinitionProviders;
            _interceptorProvider = interceptorProvider;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<HostedGrpcServer>();
            _loggerFactory = loggerFactory;
            _lifetimes = lifetimes;
            _contextAccessor = contextAccessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            GrpcEnvironment.SetLogger(new LogAdapter(_loggerFactory));
            _server = BuildServer();

            _logger.LogInformation("Grpc server starting");
            _server.Start();

            var context = new GrpcServerContext(_server);
            var contextAccessor = _contextAccessor as GrpcServerContextAccessor;
            if (contextAccessor != null)
                contextAccessor.Context = context;

            foreach (var lifetime in _lifetimes)
            {
                await lifetime.StartedAsync(cancellationToken);
            }

            _logger.LogInformation("Grpc server started");

            if (_server.Ports.Count() > 0)
            {
                var listening = _server.Ports.Select(p => $"{p.Host}:{p.BoundPort}").Aggregate((ports, port) => $"{ports}; {port}");
                _logger.LogInformation($"Listening: {listening}");
            }
        }

        private Server BuildServer()
        {
            var builder = new ServerBuilder();
            builder.AddServiceDefinition(_serviceDefinitionProvider.GetServiceDefinitions())
               .UseInterceptor(_interceptorProvider.GetInterceptors());

            if (_options == null)
            {
                builder.AutoPort();
                return builder.Build();
            }

            if (_options.ServerPorts?.Count < 1)
            {
                builder.AutoPort();
            }
            else
            {
                _options.ServerPorts.ForEach(p => builder.AddPort(p));
            }

            _options.ChannelOptions?.ForEach(o => builder.AddChannelOption(o));

            return builder.Build();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Grpc server stopping");

            var context = new GrpcServerContext(_server);
            foreach (var lifetime in _lifetimes.Reverse())
            {
                await lifetime.StoppingAsync(cancellationToken);
            }

            if (_server != null)
            {
                await _server.ShutdownAsync();
            }

            _logger.LogInformation("Grpc server stopped");
        }
    }
}