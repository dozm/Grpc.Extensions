using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ConsulClientOptions
    {
        public string Address { get; set; }

        /// <summary>
        ///  grpc client and consul mapping
        /// </summary>
        public Dictionary<Type, string> ServiceMap { get; set; } = new Dictionary<Type, string>();
    }
}
