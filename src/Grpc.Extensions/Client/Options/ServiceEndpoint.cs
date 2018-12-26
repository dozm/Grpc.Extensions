namespace Grpc.Extensions.Client.Options
{
    public class ServiceEndpoint
    {
        public string ServiceName { get; set; }

        public string[] Endpoints { get; set; }
    }
}