using Grpc.Core;

namespace Grpc.Extensions
{
    public interface IServiceDefinitionFactory
    {
        ServerServiceDefinition Create();
    }
}