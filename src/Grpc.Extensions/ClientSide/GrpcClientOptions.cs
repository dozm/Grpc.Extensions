using System;
using System.Collections.Generic;

namespace Grpc.Extensions.ClientSide
{
    public class GrpcClientOptions
    {
        public List<ClientMetadata> Clients { get; set; } = new List<ClientMetadata>();

        //public Dictionary<string, List<ServiceEndPoint>> ServicesEndpoint = new Dictionary<string, List<ServiceEndPoint>>();
        public Dictionary<Type, List<ServiceEndPoint>> ServicesEndpoint = new Dictionary<Type, List<ServiceEndPoint>>();
    }
}