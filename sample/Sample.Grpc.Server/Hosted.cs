using Grpc.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Grpc.Server
{
    public class Hosted : IHostedService
    {
        private readonly IServiceDefinitionProvider _serviceDefinitionProvider;
        private readonly IInterceptorProvider _interceptorProvider;
        private readonly ILogger<Hosted> _logger;

        public Hosted(IServiceDefinitionProvider serviceDefinitionProvider,
            IInterceptorProvider interceptorProvider,
            ILogger<Hosted> logger)
        {
            _serviceDefinitionProvider = serviceDefinitionProvider;

            _interceptorProvider = interceptorProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");
            var builder = new ServerBuilder();

            var server = builder.AddServiceDefinition(_serviceDefinitionProvider.GetServiceDefinitions())
                .AddPort(9001)
                .UseInterceptor(_interceptorProvider.GetInterceptors())
                .Build();

            server.Start();

            _logger.LogInformation("Started");

            var listening = server.Ports.Select(p => $"{p.Host}:{p.BoundPort}").Aggregate((ports, port) => $"{ports}; {port}");
            _logger.LogInformation($"Listening: {listening}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
        }
    }
}