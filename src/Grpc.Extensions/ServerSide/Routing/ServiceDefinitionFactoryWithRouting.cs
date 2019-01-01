using Google.Protobuf.Reflection;
using Grpc.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Grpc.Extensions.ServerSide.Routing
{
    public class ServiceDefinitionFactoryWithRouting<TService> : IServiceDefinitionFactory
    {
        private readonly RoutingHandler _routingHandler;

        public ServiceDefinitionFactoryWithRouting(RoutingHandler routingHandler)
        {
            _routingHandler = routingHandler;
        }

        public ServerServiceDefinition Create()
        {
            var builder = ServerServiceDefinition.CreateBuilder();
            var declaringType = typeof(TService).BaseType.DeclaringType;

            var serviceDescriptor = (ServiceDescriptor)declaringType.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            var methodDescriptors = serviceDescriptor.Methods;

            var methods = GetMethods(declaringType);

            foreach (var methodDescriptor in methodDescriptors)
            {
                var method = methods.First(m => m.Name == methodDescriptor.Name);

                var handlerMehod = CreateHandlerMethod(methodDescriptor);

                var handlerDelegateType = typeof(UnaryServerMethod<,>).MakeGenericType(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);

                var handler = handlerMehod.CreateDelegate(handlerDelegateType, _routingHandler);

                CreateAddMethod(methodDescriptor).Invoke(builder, new object[] { method, handler });
            }

            return builder.Build();
        }

        private MethodInfo CreateAddMethod(MethodDescriptor methodDescriptor)
        {
            var builderType = typeof(ServerServiceDefinition.Builder);
            var methodInfo = builderType.GetTypeInfo().DeclaredMethods.FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length > 1 && ps[1].ParameterType.GetGenericTypeDefinition() == typeof(UnaryServerMethod<,>);
            });

            return methodInfo.MakeGenericMethod(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);
        }

        private MethodInfo CreateHandlerMethod(MethodDescriptor methodDescriptor)
        {
            var handlerType = typeof(RoutingHandler);
            var handlerMethod = handlerType.GetMethod("Unary", BindingFlags.Public | BindingFlags.Instance);
            return handlerMethod.MakeGenericMethod(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);
        }

        private IMethod[] GetMethods(Type type)
        {
            var feildType = typeof(IMethod);
            var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(p => feildType.IsAssignableFrom(p.FieldType));

            return props.Select(p => (IMethod)p.GetValue(null)).ToArray();
        }
    }
}