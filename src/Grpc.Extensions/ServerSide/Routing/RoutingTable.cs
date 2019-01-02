using Microsoft.Extensions.Internal;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Grpc.Extensions.ServerSide.Routing
{
    public interface IRoutingTable
    {
        void Register(string routingKey, ObjectMethodExecutor executor);

        ObjectMethodExecutor GetExecutor(string routingKey);
    }

    public class RoutingTable : IRoutingTable
    {
        private readonly ConcurrentDictionary<string, ObjectMethodExecutor> Executors = new ConcurrentDictionary<string, ObjectMethodExecutor>();

        public ObjectMethodExecutor GetExecutor(string routingKey)
        {
            if (Executors.TryGetValue(routingKey, out var executor))
            {
                return executor;
            }

            throw new KeyNotFoundException($"路由 key {routingKey} 没有匹配的执行器。");
        }

        public void Register(string routingKey, ObjectMethodExecutor executor)
        {
            Executors[routingKey] = executor;
        }
    }
}