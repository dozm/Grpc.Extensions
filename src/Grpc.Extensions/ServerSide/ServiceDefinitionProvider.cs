using Grpc.Core;
using System.Collections.Generic;
using System.Linq;

namespace Grpc.Extensions.Internal
{
    internal class ServiceDefinitionProvider : IServiceDefinitionProvider
    {
        private readonly IEnumerable<IServiceDefinitionFactory> _factories;

        public ServiceDefinitionProvider(IEnumerable<IServiceDefinitionFactory> factories)
        {
            _factories = factories;
        }

        public IEnumerable<ServerServiceDefinition> GetServiceDefinitions()
        {
            return _factories.Select(f => f.Create());
        }
    }
}