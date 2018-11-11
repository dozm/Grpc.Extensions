using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Grpc.Extensions.Client
{
    public class ClientMetadata
    {
        private string _serviceName;
        public Type ClientType { get; }
        public string ServiceName
        {
            get
            {
                if (_serviceName == null)
                    _serviceName = GetDefaultServiceName();
                return _serviceName;
            }
        }

        public ClientMetadata(Type clientType) : this(clientType, null)
        {
        }

        public ClientMetadata(Type clientType, string serviceName)
        {
            _serviceName = serviceName;
            ClientType = clientType;
        }

        private string GetDefaultServiceName()
        {
            if (ClientType.IsNested)
            {
                var serviceNameField = ClientType.DeclaringType?.GetField("__ServiceName", BindingFlags.Static | BindingFlags.NonPublic);
                if (serviceNameField != null)
                {
                    return serviceNameField.GetValue(null) as string;
                }

            }

            throw new Exception($"Not found default service name of {ClientType}");
        }
    }
}
