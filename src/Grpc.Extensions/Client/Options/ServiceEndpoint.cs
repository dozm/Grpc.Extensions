using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client.Options
{
    public class ServiceEndpoint
    {
        public string ServiceName { get; set; }

        public string[] Endpoints { get; set; }
    }
}
