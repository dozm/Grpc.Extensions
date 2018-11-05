using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public interface IGrpcSerivce
    {
        ServerServiceDefinition BuildServiceDefinition();
    }
}
