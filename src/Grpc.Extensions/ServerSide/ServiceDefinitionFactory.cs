using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Grpc.Core;

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
                var bindServiceMethod = serviceBaseType.DeclaringType?.GetMethod("BindService", BindingFlags.Public | BindingFlags.Static);
                if (bindServiceMethod != null)
                {
                    return (ServerServiceDefinition)bindServiceMethod.Invoke(null, new object[] { _service });
                }
            }

            throw new Exception($"无法为 {serviceType} 找到 BindService 方法。");
        }



    }
}
