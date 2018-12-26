using Grpc.Core;
using System.Collections.Generic;

namespace Grpc.Extensions
{
    public interface IServiceDefinitionProvider
    {
        IEnumerable<ServerServiceDefinition> GetServiceDefinitions();
    }
}