using Grpc.Core.Interceptors;

namespace Grpc.Extensions.ServerSide.Interceptors
{
    public abstract class ServerInterceptor : Interceptor
    {
        /// <summary>
        /// 排序值，值小的在外层。
        /// </summary>
        public virtual int Order { get; set; }
    }
}