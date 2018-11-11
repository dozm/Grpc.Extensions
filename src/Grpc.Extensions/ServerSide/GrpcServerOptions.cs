using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    public class GrpcServerOptions
    {

        public List<ServerPort> ServerPorts { get; set; } = new List<ServerPort>();

        public List<ChannelOption> ChannelOptions { get; set; } = new List<ChannelOption>();

        public List<object> ServicesType { get; set; } = new List<object>();

    }
}
