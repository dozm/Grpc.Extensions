using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Linq;

namespace Grpc.Extensions.ClientSide
{
    public class CallInvokerWrapper : CallInvoker
    {
        private readonly Lazy<CallInvoker> _callInvokeLazy;
        private CallInvoker InterceptedCallInvoker => _callInvokeLazy.Value;

        public CallInvokerWrapper(StatelessCallInvoker callInvoker, IInterceptorProvider interceptorProvider)
        {
            _callInvokeLazy = new Lazy<CallInvoker>(() => callInvoker.Intercept(interceptorProvider.GetInterceptors().ToArray()), true);
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return InterceptedCallInvoker.AsyncClientStreamingCall(method, host, options);
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return InterceptedCallInvoker.AsyncDuplexStreamingCall(method, host, options);
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return InterceptedCallInvoker.AsyncServerStreamingCall(method, host, options, request);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return InterceptedCallInvoker.AsyncUnaryCall(method, host, options, request);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return InterceptedCallInvoker.BlockingUnaryCall(method, host, options, request);
        }
    }
}