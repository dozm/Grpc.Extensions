namespace Grpc.Extensions.Consul.Options
{
    public class ServiceRegistrationOptions
    {
        /// <summary>
        /// grpc 服务在 consul 中的名称。
        /// </summary>
        public string ConsulServiceName { get; set; }
        /// <summary>
        /// grpc服务地址，用于注册到consul中。
        /// 如果未设置，则自动获取 grpc server 当前监听的地址。
        /// </summary>
        public string ServiceHost { get; set; }
        public int ServicePort { get; set; }

        public bool EnableTagOverride { get; set; } = true;

        public string[] Tags { get; set; }
    }
}