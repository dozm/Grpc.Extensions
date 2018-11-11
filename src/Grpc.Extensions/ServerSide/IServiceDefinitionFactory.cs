using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public interface IServiceDefinitionFactory
    {
        ServerServiceDefinition Create();
    }
}
