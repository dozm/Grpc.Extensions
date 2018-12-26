using Google.Protobuf;
using Grpc.Core;
using Grpc.Extensions.Test.Messages;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Extensions.Test
{
    public class ServerBuilderTest
    {
        [Fact]
        public void Test1()
        {
            var builder = new ServerBuilder();
            builder.AddPort(new ServerPort("0.0.0.0", ServerPort.PickUnused, ServerCredentials.Insecure));
            builder.AddPort(new ServerPort("0.0.0.0", 5566, ServerCredentials.Insecure));
            builder.AutoPort();

            var ssdb = ServerServiceDefinition.CreateBuilder();

            Marshaller<Request1> requestMarshaller = Marshallers.Create<Request1>(
                arg => MessageExtensions.ToByteArray(arg),
                Request1.Parser.ParseFrom);

            var method = new Method<Request1, Response1>(MethodType.Unary, "testService", "testMethod", null, null);

            ssdb.AddMethod<Request1, Response1>(method, Handler);

            builder.AddServiceDefinition(ssdb.Build());

            var server = builder.Build();

            Assert.NotNull(server.Ports);
        }

        private Task<Response1> Handler(Request1 request, ServerCallContext context)
        {
            return Task.FromResult(new Response1());
        }
    }
}