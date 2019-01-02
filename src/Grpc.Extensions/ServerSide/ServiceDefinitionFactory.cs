using Grpc.Core;
using System;

namespace Grpc.Extensions
{
    public class ServiceDefinitionFactory<TService> : IServiceDefinitionFactory
    {
        private readonly TService _service;

        public ServiceDefinitionFactory(TService service)
        {
            _service = service;
        }

        public ServerServiceDefinition Create()
        {
            var serviceType = _service.GetType();
            var serviceBaseType = serviceType.BaseType;

            if (serviceBaseType != null && serviceBaseType.IsNested)
            {
                var bindServiceMethod = serviceBaseType.DeclaringType?.GetMethod("BindService", new Type[] { serviceBaseType });

                //bindServiceMethod = serviceBaseType.DeclaringType.GetMethods().First(m => m.Name == "BindService" && m.GetParameters().Length == 2);

                if (bindServiceMethod != null)
                {
                    return (ServerServiceDefinition)bindServiceMethod.Invoke(null, new object[] { _service });
                }
            }

            throw new Exception($"无法为 {serviceType} 找到 BindService 方法。");
        }
    }
}