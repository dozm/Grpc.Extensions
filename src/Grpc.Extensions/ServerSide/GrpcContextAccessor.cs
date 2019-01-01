using System.Threading;

namespace Grpc.Extensions.ServerSide
{
    public interface IGrpcContextAccessor
    {
        GrpcContext GrpcContext { get; set; }
    }

    public class GrpcContextAccessor : IGrpcContextAccessor
    {
        private static AsyncLocal<GrpcContextHolder> _grpcContextCurrent = new AsyncLocal<GrpcContextHolder>();

        public GrpcContext GrpcContext
        {
            get { return _grpcContextCurrent.Value?.Context; }

            set
            {
                var holder = _grpcContextCurrent.Value;
                if (holder != null)
                {
                    holder.Context = null;
                }

                if (value != null)
                {
                    _grpcContextCurrent.Value = new GrpcContextHolder { Context = value };
                }
            }
        }

        private class GrpcContextHolder
        {
            public GrpcContext Context;
        }
    }
}