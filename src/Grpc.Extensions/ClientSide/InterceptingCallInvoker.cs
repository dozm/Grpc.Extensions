using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extensions.ClientSide.Interceptors;
using System;
using System.Linq;

namespace Grpc.Extensions.ClientSide
{
    public class InterceptingCallInvoker : CallInvoker
    {
        private readonly Lazy<CallInvoker> _callInvokeLazy;
        private CallInvoker CallInvoker => _callInvokeLazy.Value;

        public InterceptingCallInvoker(StatelessCallInvoker callInvoker, IClientInterceptorProvider interceptorProvider)
        {
            _callInvokeLazy = new Lazy<CallInvoker>(() => callInvoker.Intercept(interceptorProvider.GetInterceptors().ToArray()), true);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return CallInvoker.AsyncClientStreamingCall(method, host, options);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return CallInvoker.AsyncDuplexStreamingCall(method, host, options);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return CallInvoker.AsyncServerStreamingCall(method, host, options, request);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return CallInvoker.AsyncUnaryCall(method, host, options, request);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return CallInvoker.BlockingUnaryCall(method, host, options, request);
        }
    }
}