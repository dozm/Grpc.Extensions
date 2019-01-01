using System;
using System.Collections.Generic;

namespace Grpc.Extensions.Consul.ClientSide
{
    public class ConsulClientOptions
    {
        public string Address { get; set; }

        /// <summary>
        /// grpc客户端类型和consul服务名称的映射。
        /// </summary>
        public ConsulServiceMaps ServiceMaps { get; set; } = new ConsulServiceMaps();
    }

    public class ConsulServiceMaps
    {
        public Dictionary<Type, string> Maps { get; set; } = new Dictionary<Type, string>();

        public void Add<TGrpcClient>(string consulServiceName)
        {
            Maps[typeof(TGrpcClient)] = consulServiceName;
        }

        public string Get(Type grpcClientType)
        {
            return Maps[grpcClientType];
        }
    }
}