using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core.Interceptors;
using static Sample.Services.Service1;
using static Sample.Services.Service2;
using Sample.Messages;

namespace Sample.Grpc.Server
{
    public class Hosted : IHostedService
    {

        private readonly ILogger<Hosted> _logger;
        private readonly Service1Client _service1Client;
        private readonly Service2Client _service2Client;
        private readonly IEnumerable<Interceptor> _interceptors;

        public Hosted(
            IEnumerable<Interceptor> interceptors,
            Service1Client service1Client,
            Service2Client service2Client,
            ILogger<Hosted> logger)
        {
            _service1Client = service1Client;
            _service2Client = service2Client;
            _interceptors = interceptors;

            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");


            _logger.LogWarning(_service1Client.API1(new Request1() { Message = "Hi, Server1." }).Message);
            _logger.LogWarning(_service2Client.API2(new Request1 { Message = "Hi,Servie2." }).Message);

            _logger.LogWarning(_service1Client.API1Async(new Request1() { Message = "Hi, Server." }).GetAwaiter().GetResult().Message);


            _logger.LogInformation($"Started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
        }
    }
}
