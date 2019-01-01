using System;
using System.Collections.Generic;

namespace Grpc.Extensions.ClientSide
{
    public class GrpcClientOptions
    {
        public List<ClientMetadata> Clients { get; set; } = new List<ClientMetadata>();

        /// <summary>
        /// grpc 客户端和服务地址映射。
        /// </summary>
        public GrpcServiceMaps ServiceMaps { get; set; } = new GrpcServiceMaps();
    }

    public class GrpcServiceMaps
    {
        public Dictionary<Type, List<ServiceEndPoint>> Maps = new Dictionary<Type, List<ServiceEndPoint>>();

        public void Add<TGrpcClient>(string host, int port)
        {
            var clientType = typeof(TGrpcClient);
            if (!Maps.TryGetValue(clientType, out var endports))
            {
                endports = new List<ServiceEndPoint>();
                Maps.Add(clientType, endports);
            }

            endports.Add(new ServiceEndPoint { Host = host, Port = port });
        }

        public ServiceEndPoint[] Get(Type clientType)
        {
            if (!Maps.TryGetValue(clientType, out var endports))
            {
                endports = null;
            }

            return endports.ToArray();
        }
    }
}