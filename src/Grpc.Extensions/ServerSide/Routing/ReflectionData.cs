using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grpc.Extensions.ServerSide.Routing
{
    internal class ReflectionData
    {
        public static Dictionary<MethodType, Type> GrpcServerMethodDelegateTypes => new Dictionary<MethodType, Type>
        {
            { MethodType.Unary, typeof(UnaryServerMethod<,>)},
            { MethodType.ClientStreaming, typeof(ClientStreamingServerMethod<,>)},
            { MethodType.ServerStreaming, typeof(ServerStreamingServerMethod<,>)},
            { MethodType.DuplexStreaming, typeof(DuplexStreamingServerMethod<,>)}
        };

        public static Dictionary<MethodType, MethodInfo> ServiceDefinitionAddMethods { get; }

        public static Dictionary<MethodType, MethodInfo> RoutingHandleMethods { get; }

        static ReflectionData()
        {
            var routerType = typeof(Router);
            RoutingHandleMethods = new Dictionary<MethodType, MethodInfo>
            {
                { MethodType.Unary, routerType.GetMethod(nameof(Router.Unary))},
                { MethodType.ClientStreaming, routerType.GetMethod(nameof(Router.ClientStreaming))},
                { MethodType.ServerStreaming, routerType.GetMethod(nameof(Router.ServerStreaming))},
                { MethodType.DuplexStreaming, routerType.GetMethod(nameof(Router.DuplexStreaming))}
            };

            var addMethods = typeof(ServerServiceDefinition.Builder).GetTypeInfo().DeclaredMethods.Where(m => m.Name == "AddMethod");
            ServiceDefinitionAddMethods = new Dictionary<MethodType, MethodInfo>
            {
                { MethodType.Unary, addMethods.First(mi => mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == GrpcServerMethodDelegateTypes[MethodType.Unary])},
                { MethodType.ClientStreaming, addMethods.First(mi => mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == GrpcServerMethodDelegateTypes[MethodType.ClientStreaming])},
                { MethodType.ServerStreaming, addMethods.First(mi => mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == GrpcServerMethodDelegateTypes[MethodType.ServerStreaming])},
                { MethodType.DuplexStreaming, addMethods.First(mi => mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == GrpcServerMethodDelegateTypes[MethodType.DuplexStreaming])}
            };
        }
    }
}