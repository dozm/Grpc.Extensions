using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Grpc.Extensions
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddPort(this ServerBuilder builder, string host, int port, ServerCredentials credentials)
        {
            builder.AddPort(new ServerPort(host, port, credentials));
            return builder;
        }

        public static ServerBuilder AddPort(this ServerBuilder builder, string host, int port)
        {
            return builder.AddPort(host, port, ServerCredentials.Insecure);
        }

        public static ServerBuilder AddPort(this ServerBuilder builder, int port)
        {
            return builder.AddPort($"[{IPAddress.IPv6Any}]", port, ServerCredentials.Insecure);
        }

        public static ServerBuilder AutoPort(this ServerBuilder builder, string host)
        {
            return builder.AddPort(host, ServerPort.PickUnused);
        }
        public static ServerBuilder AutoPort(this ServerBuilder builder)
        {
            return builder.AddPort(ServerPort.PickUnused);
        }

        public static ServerBuilder AddServiceDefinition(this ServerBuilder builder, IEnumerable<ServerServiceDefinition> serviceDefinitions)
        {
            foreach (var s in serviceDefinitions)
            {
                builder.AddServiceDefinition(s);
            }
            return builder;
        }

        public static ServerBuilder UseInterceptor(this ServerBuilder builder, IEnumerable<Interceptor> interceptors)
        {

            foreach(var i in interceptors)
            {
                builder.UseInterceptor(i);
            }

            return builder;
        }

        public static ServerBuilder AddChannelOption(this ServerBuilder builider,IEnumerable<ChannelOption> options)
        {
            foreach (var o in options)
            {
                builider.AddChannelOption(o);
            }

            return builider;
        }
    }
}
