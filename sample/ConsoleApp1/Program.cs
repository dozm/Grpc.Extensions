using Grpc.Extensions.ClientSide;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        private static IServiceProvider Services;
        private static ILogger<Program> log;

        private static void Main(string[] args)
        {
            ServiceCollection service = new ServiceCollection();

            service.AddLogging(b =>
            {
                b.SetMinimumLevel(LogLevel.Warning);
                b.AddConsole();
            });

            Services = service.BuildServiceProvider();

            log = Services.GetService<ILogger<Program>>();

            Test1();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        public static void Test1()
        {
            var channelFactory = Services.GetService<IChannelFactory>();
            LoadBalancer lb = new LoadBalancer("svc1", new FakeServiceDiscovery(), channelFactory, Services.GetService<ILoggerFactory>());

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = Parallel.For(0, 100000, i =>
            {
                try
                {
                    var a = lb.SelectChannel();
                    log.LogInformation($"{a.Target}");
                    //Task.Delay(100).Wait();
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "选择通道时发生异常");
                }

                //var task = lb.TryUpdateAsync();
                //var x = task.Wait(7000);
                //log.LogInformation($"{x}  {i}");
            });

            log.LogWarning($"{result.IsCompleted}");
            sw.Stop();
            log.LogWarning($"时间：{sw.Elapsed}");
        }
    }

    public class FakeServiceDiscovery : IServiceDiscovery
    {
        private int count = 0;

        public async Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken)
        {
            await Task.Delay(5000);
            Interlocked.Increment(ref count);

            if (count % 3 == 0)
                throw new Exception($"未发现服务终端 {serviceName}");

            return new ServiceEndPoint[]
            {
                new ServiceEndPoint(){ Address="127.0.0.1" , Port=80},
                new ServiceEndPoint(){ Address="172.20.10.12" , Port=80}
            };
        }
    }
}