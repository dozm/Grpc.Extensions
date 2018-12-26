using Grpc.Extensions.Client.Options;
using System.Collections.Generic;

namespace Grpc.Extensions.Client
{
    public class GrpcClientOptions
    {
        public List<ClientMetadata> Clients { get; set; } = new List<ClientMetadata>();

        public Dictionary<string, ServiceEndpoint> ServicesEndpoint = new Dictionary<string, ServiceEndpoint>();
    }
}