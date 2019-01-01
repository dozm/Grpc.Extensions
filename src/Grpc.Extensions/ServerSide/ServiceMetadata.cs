using Google.Protobuf.Reflection;
using System;
using System.Reflection;

namespace Grpc.Extensions.ServerSide
{
    public class ServiceMetadata
    {
        public Type ServiceType { get; }

        private ServiceDescriptor _descriptor;

        public ServiceDescriptor Descriptor
        {
            get
            {
                if (_descriptor == null)
                    _descriptor = GetDescriptor();
                return _descriptor;
            }
        }

        public string ServiceName => Descriptor.FullName;

        public ServiceMetadata(Type serviceType)
        {
            ServiceType = serviceType;
        }

        private ServiceDescriptor GetDescriptor()
        {
            var declaringType = ServiceType.BaseType.DeclaringType;
            return (ServiceDescriptor)declaringType.GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }
    }
}