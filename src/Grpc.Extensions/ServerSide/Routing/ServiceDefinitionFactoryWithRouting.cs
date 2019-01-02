using Google.Protobuf.Reflection;
using Grpc.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Grpc.Extensions.ServerSide.Routing
{
    public class ServiceDefinitionFactoryWithRouting<TService> : IServiceDefinitionFactory
    {
        private readonly Router _router;

        public ServiceDefinitionFactoryWithRouting(Router router)
        {
            _router = router;
        }

        public ServerServiceDefinition Create()
        {
            var builder = ServerServiceDefinition.CreateBuilder();
            var declaringType = typeof(TService).BaseType.DeclaringType;

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