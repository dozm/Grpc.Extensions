using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public interface IServiceDefinitionProvider
    {
        IEnumerable<ServerServiceDefinition> GetServiceDefinitions();
    }
}
