using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Extensions.Internal
{
    internal class ServiceDefinitionProvider : IServiceDefinitionProvider
    {
        private readonly IEnumerable<IGrpcSerivce> _grpcServices;

        public ServiceDefinitionProvider(IEnumerable<IGrpcSerivce> grpcServices)
        {
            _grpcServices = grpcServices;
        }
        public IEnumerable<ServerServiceDefinition> GetServiceDefinitions()
        {

            return _grpcServices.Select(s => s.BuildServiceDefinition());
        }
    }
}
