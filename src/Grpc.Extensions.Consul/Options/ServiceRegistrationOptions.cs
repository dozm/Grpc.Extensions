using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.Options
{
    public class ServiceRegistrationOptions
    {
        /// <summary>
        /// grpc 服务在 consul 中的名称。
        /// </summary>
        public string ConsulServiceName { get; set; }

        public bool EnableTagOverride { get; set; } = true;
    }
}
