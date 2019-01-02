using Google.Protobuf.Reflection;
using Grpc.Core;
using Microsoft.Extensions.Internal;
using System;
using System.Linq;
using System.Reflection;

namespace Grpc.Extensions.ServerSide.Routing
{
    public class ServiceDefinitionFactoryWithRouting<TService> : IServiceDefinitionFactory
    {
        private readonly Router _router;
        private readonly IRoutingTable _routingTable;

        public ServiceDefinitionFactoryWithRouting(Router router, IRoutingTable routingTable)
        {
            _router = router;
            _routingTable = routingTable;
        }

        public ServerServiceDefinition Create()
        {
            var builder = ServerServiceDefinition.CreateBuilder();
            var serviceType = typeof(TService);
            var declaringType = serviceType.BaseType.DeclaringType;

            var serviceDescriptor = (ServiceDescriptor)declaringType.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null);

            var methodDescriptors = serviceDescriptor.Methods;

            var methods = GetMethods(declaringType);
            MethodType methodType;
            MethodInfo handlerMehodInfo;
            IMethod method;

            foreach (var methodDescriptor in methodDescriptors)
            {
                var handlerType = typeof(Router);
                methodType = methodDescriptor.GetMethodType();
                method = methods.First(m => m.Name == methodDescriptor.Name);

                handlerMehodInfo = ReflectionData.RoutingHandleMethods[methodType].MakeGenericMethod(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType);

                var handler = handlerMehodInfo.CreateDelegate(
                    ReflectionData.GrpcServerMethodDelegateTypes[methodType].MakeGenericType(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType),
                    _router);

                // 添加服务定义。
                ReflectionData.ServiceDefinitionAddMethods[methodType]
                    .MakeGenericMethod(methodDescriptor.InputType.ClrType, methodDescriptor.OutputType.ClrType)
                    .Invoke(builder, new object[] { method, handler });

                // 注册执行器，用于执行实际的 grpc 服务方法。
                _routingTable.Register($"/{serviceDescriptor.FullName}/{methodDescriptor.Name}",
                    ObjectMethodExecutor.Create(serviceType.GetMethod(methodDescriptor.Name), serviceType.GetTypeInfo()));
            }

            return builder.Build();
        }

        private static IMethod[] GetMethods(Type type)
        {
            var feildType = typeof(IMethod);
            var props = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(p => feildType.IsAssignableFrom(p.FieldType));

            return props.Select(p => (IMethod)p.GetValue(null)).ToArray();
        }
    }
}