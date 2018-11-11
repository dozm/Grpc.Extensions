using Grpc.Extensions.Client.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Client
{
    public class GrpcClientOptions
    {
        public List<ClientMetadata> Clients { get; set; } = new List<ClientMetadata>();

        public Dictionary<string, ServiceEndpoint> ServicesEndpoint = new Dictionary<string, ServiceEndpoint>();
    }
}
