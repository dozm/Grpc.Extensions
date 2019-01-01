using Grpc.Core.Interceptors;

namespace Grpc.Extensions.ClientSide.Interceptors
{
    public abstract class ClientInterceptor : Interceptor
    {
        /// <summary>
        /// 排序值，值小的在外层。
        /// </summary>
        public virtual int Order { get; set; }
    }
}