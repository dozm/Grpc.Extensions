namespace Grpc.Extensions.Consul.Options
{
    public class ServiceRegistrationOptions
    {
        /// <summary>
        /// grpc 服务在 consul 中的名称。
        /// </summary>
        public string ConsulServiceName { get; set; }

        public string ServiceHost { get; set; }
        public int ServicePort { get; set; }

        public bool EnableTagOverride { get; set; } = true;

        public string[] Tags { get; set; }
    }
}