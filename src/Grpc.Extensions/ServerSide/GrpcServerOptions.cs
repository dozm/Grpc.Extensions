using Grpc.Core;
using Grpc.Extensions.ServerSide;
using System.Collections.Generic;

namespace Grpc.Extensions
{
    public class GrpcServerOptions
    {
        public List<ServerPort> ServerPorts { get; set; } = new List<ServerPort>();

        public List<ChannelOption> ChannelOptions { get; set; } = new List<ChannelOption>();

        public List<ServiceMetadata> ServiceMetadatas { get; set; } = new List<ServiceMetadata>();
    }
}