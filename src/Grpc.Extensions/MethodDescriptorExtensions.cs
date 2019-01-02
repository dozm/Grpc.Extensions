using Google.Protobuf.Reflection;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions
{
    internal static class MethodDescriptorExtensions
    {
        public static MethodType GetMethodType(this MethodDescriptor methodDescriptor)
        {
            if (!methodDescriptor.IsClientStreaming && !methodDescriptor.IsServerStreaming)
                return MethodType.Unary;
            else if (methodDescriptor.IsClientStreaming && methodDescriptor.IsServerStreaming)
                return MethodType.DuplexStreaming;
            else if (methodDescriptor.IsClientStreaming)
                return MethodType.ClientStreaming;
            else
                return MethodType.ServerStreaming;

        }
    }
}
