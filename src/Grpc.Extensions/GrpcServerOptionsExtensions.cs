using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Grpc.Extensions
{
    public static class GrpcServerOptionsExtensions
    {

        public static GrpcServerOptions AddPort(this GrpcServerOptions options, string host, int port, ServerCredentials credentials)
        {
            options.ServerPorts.Add(new ServerPort(host, port, credentials));
            return options;
        }

        public static GrpcServerOptions AddPort(this GrpcServerOptions options, string host, int port)
        {
            return options.AddPort(host, port, ServerCredentials.Insecure);
        }

        public static GrpcServerOptions AddPort(this GrpcServerOptions options, int port)
        {
            return options.AddPort($"[{IPAddress.IPv6Any}]", port);
        }

        public static GrpcServerOptions AutoPort(this GrpcServerOptions options, string host)
        {
            return options.AddPort(host, ServerPort.PickUnused);
        }
        public static GrpcServerOptions AutoPort(this GrpcServerOptions options)
        {
            return options.AddPort(ServerPort.PickUnused);
        }


        public static GrpcServerOptions AddChannelOption(this GrpcServerOptions options, string name, string value)
        {
            options.ChannelOptions.Add(new ChannelOption(name, value));
            return options;
        }

        public static GrpcServerOptions AddChannelOption(this GrpcServerOptions options, string name, int value)
        {
            options.ChannelOptions.Add(new ChannelOption(name, value));
            return options;
        }
    }
}
