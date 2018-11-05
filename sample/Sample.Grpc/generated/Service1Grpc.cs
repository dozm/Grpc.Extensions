// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Service1.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Sample.Services {
  public static partial class Service1
  {
    static readonly string __ServiceName = "Sample.GrpcService.Service1";

    static readonly grpc::Marshaller<global::Sample.Messages.Request1> __Marshaller_Sample_GrpcService_Request1 = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sample.Messages.Request1.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sample.Messages.Response1> __Marshaller_Sample_GrpcService_Response1 = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sample.Messages.Response1.Parser.ParseFrom);

    static readonly grpc::Method<global::Sample.Messages.Request1, global::Sample.Messages.Response1> __Method_API1 = new grpc::Method<global::Sample.Messages.Request1, global::Sample.Messages.Response1>(
        grpc::MethodType.Unary,
        __ServiceName,
        "API1",
        __Marshaller_Sample_GrpcService_Request1,
        __Marshaller_Sample_GrpcService_Response1);

    static readonly grpc::Method<global::Sample.Messages.Request1, global::Sample.Messages.Response1> __Method_API2 = new grpc::Method<global::Sample.Messages.Request1, global::Sample.Messages.Response1>(
        grpc::MethodType.Unary,
        __ServiceName,
        "API2",
        __Marshaller_Sample_GrpcService_Request1,
        __Marshaller_Sample_GrpcService_Response1);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Sample.Services.Service1Reflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of Service1</summary>
    public abstract partial class Service1Base
    {
      public virtual global::System.Threading.Tasks.Task<global::Sample.Messages.Response1> API1(global::Sample.Messages.Request1 request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::Sample.Messages.Response1> API2(global::Sample.Messages.Request1 request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Service1</summary>
    public partial class Service1Client : grpc::ClientBase<Service1Client>
    {
      /// <summary>Creates a new client for Service1</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public Service1Client(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Service1 that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public Service1Client(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected Service1Client() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected Service1Client(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::Sample.Messages.Response1 API1(global::Sample.Messages.Request1 request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return API1(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sample.Messages.Response1 API1(global::Sample.Messages.Request1 request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_API1, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sample.Messages.Response1> API1Async(global::Sample.Messages.Request1 request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return API1Async(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sample.Messages.Response1> API1Async(global::Sample.Messages.Request1 request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_API1, null, options, request);
      }
      public virtual global::Sample.Messages.Response1 API2(global::Sample.Messages.Request1 request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return API2(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sample.Messages.Response1 API2(global::Sample.Messages.Request1 request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_API2, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sample.Messages.Response1> API2Async(global::Sample.Messages.Request1 request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return API2Async(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sample.Messages.Response1> API2Async(global::Sample.Messages.Request1 request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_API2, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override Service1Client NewInstance(ClientBaseConfiguration configuration)
      {
        return new Service1Client(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(Service1Base serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_API1, serviceImpl.API1)
          .AddMethod(__Method_API2, serviceImpl.API2).Build();
    }

  }
}
#endregion
