using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extensions.Consul.ClientSide;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var task = Task.Run(() => Run());

            _logger.LogInformation($"Started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
        }

        private async Task Run(CancellationToken token = default)
        {
            await Task.Delay(100);
            do
            {
                _logger.LogInformation("==============================================");
                try
                {
                    LogResponse(await _service1Client.API1Async(new Request1() { Message = "Hi, Server1." },
                         options: new CallOptions(cancellationToken: token)));


                    //await ClientStreamRequest();

                    //await DuplexStreamingRequest();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "");
                }

            } while (Console.ReadLine() == "");
        }

        private void LogResponse(Response1 response)
        {
            _logger.LogInformation(response.Message);
        }

        private async Task ClientStreamRequest()
        {
            using (var call = _service1Client.ClientStreamAPI())
            {
                foreach (var character in "Hello world.")
                {
                    await call.RequestStream.WriteAsync(new Request1 { Message = character.ToString() });
                }
                await call.RequestStream.CompleteAsync();
              
                var response = await call.ResponseAsync;
                _logger.LogInformation($"响应：{response.Message}");
            }
        }

        private async Task DuplexStreamingRequest()
        {
            using (var call = _service1Client.DuplexStreamingAPI())
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var msg = call.ResponseStream.Current.Message;
                        _logger.LogInformation($"收到响应： {msg}");
                    }
                });

                foreach (var question in new string[] { "Who are you ?", "Where are you from ?", "Where are you going ?" })
                {
                    await call.RequestStream.WriteAsync(new Request1 { Message = question });
                }
                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            }
        }
    }
}