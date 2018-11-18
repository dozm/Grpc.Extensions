using Grpc.Extensions.Consul.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul
{
    public class ConsulOptions
    {
        public string Address { get; set; }

        public ServiceRegistrationOptions ServiceRegistration { get; set; }


    }
}
