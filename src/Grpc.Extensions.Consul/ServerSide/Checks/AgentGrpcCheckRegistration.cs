using Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class AgentGrpcCheckRegistration : AgentCheckRegistration
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GRPC { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? GrpcUseTLS { get; set; }
    }
}
