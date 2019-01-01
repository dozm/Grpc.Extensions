using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Grpc.Extensions.ClientSide
{
    public class GrpcClientOptionsConfigurator : IConfigureOptions<GrpcClientOptions>
    {
        readonly private IConfiguration _config;

        public GrpcClientOptionsConfigurator(IConfiguration config)
        {
            _config = config;
        }

        public void Configure(GrpcClientOptions options)
        {
            var grpcClientConfig = _config.GetSection("GrpcClientOptions");
            if (grpcClientConfig == null)
                return;

            BindServices(options, grpcClientConfig);
        }

        private void BindServices(GrpcClientOptions options, IConfiguration config)
        {
            //var dic = config.GetSection("Services")?.Get<Dictionary<string, ServiceEndPoint[]>>();
            //var servicesEndpoint = options.ServicesEndpoint;
            //if (dic != null)
            //{
            //    foreach (var kvp in dic)
            //    {
            //        if (!servicesEndpoint.TryGetValue(kvp.Key, out var v))
            //        {
            //            v = new List<ServiceEndPoint>();
            //            servicesEndpoint.Add(kvp.Key, v);
            //        }
            //        v.AddRange(kvp.Value);
            //    }
            //}
        }
    }
}