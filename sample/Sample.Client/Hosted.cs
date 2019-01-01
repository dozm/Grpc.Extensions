﻿using Grpc.Core.Interceptors;
using Grpc.Extensions.Consul.ClientSide;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Sample.Services.Service1;
using static Sample.Services.Service2;

namespace Sample.Grpc.Server
{
    public class Hosted : IHostedService
    {
        private readonly ILogger<Hosted> _logger;
        private readonly Service1Client _service1Client;
        private readonly Service2Client _service2Client;
        private readonly IEnumerable<Interceptor> _interceptors;
        private readonly ConsulClientOptions _options;

        public Hosted(
            IEnumerable<Interceptor> interceptors,
            Service1Client service1Client,
            Service2Client service2Client,
            IOptions<ConsulClientOptions> options,
            ILogger<Hosted> logger)
        {
            _service1Client = service1Client;
            _service2Client = service2Client;
            _interceptors = interceptors;
            _options = options.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            var task = Run();

            _logger.LogInformation($"Started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
        }

        private async Task Run()
        {
            do
            {
                try
                {
                    var resp = await _service1Client.API1Async(new Request1() { Message = "111111111111  Hi, Server1." });
                    _logger.LogWarning(resp.Message);
                    //_logger.LogWarning(_service2Client.API2(new Request1 { Message = "222222222222 Hi,Servie2." }).Message);
                    //_logger.LogWarning(_service1Client.API1Async(new Request1() { Message = "33333333333333 Hi, Server." }).GetAwaiter().GetResult().Message);
                }
                catch
                {
                }

                _logger.LogInformation("================================================");
            } while (Console.ReadLine() == "");
        }
    }
}