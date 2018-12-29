using Grpc.Extensions.ClientSide;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Grpc.Extensions.Consul.Test
{
    public class LoadBalancerTest
    {
        private readonly ITestOutputHelper _output;

        public LoadBalancerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            //LoadBalancer lb = new LoadBalancer("svc1", new FakeServiceDiscovery(), null);

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //var result = Parallel.For(0, 10000, i =>
            //{
            //    var task = lb.TryUpdateAsync();
            //    _output.WriteLine($"{task.GetHashCode()},  {task.IsCompleted}  {i}");
            //    task.Wait();
            //});

            //_output.WriteLine($"{result.IsCompleted}");
            //sw.Stop();
            //_output.WriteLine($"ʱ�䣺{sw.Elapsed}");
        }
    }

    public class FakeServiceDiscovery : IServiceDiscovery
    {
        public async Task<ServiceEndPoint[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken)
        {
            await Task.Delay(5000);

            return new ServiceEndPoint[]
            {
                new ServiceEndPoint(){ Address="127.0.0.1" , Port=80},
                new ServiceEndPoint(){ Address="172.20.10.12" , Port=80}
            };
        }
    }
}