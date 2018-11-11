using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class TTLChecker
    {
        private readonly IConsulClientFactory _consulClientFactory;

        public TTLChecker(IConsulClientFactory consulClientFactory)
        {
            _consulClientFactory = consulClientFactory;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var task = new Task(PassTTL, cancellationToken, TaskCreationOptions.LongRunning).ConfigureAwait(false);

            await task;

        }

        private void PassTTL()
        {
            
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

        }
    }
}
