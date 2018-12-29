using Grpc.Core;

namespace Grpc.Extensions.ClientSide
{
    public class ChannelFactory : IChannelFactory
    {
        public ChannelFactory()
        {
        }

        public Channel Create(ServiceEndPoint serviceEndPoint)
        {
            // TODO:根据配置选项创建 channel.
            return new Channel(serviceEndPoint.ToString(), ChannelCredentials.Insecure);
        }
    }
}