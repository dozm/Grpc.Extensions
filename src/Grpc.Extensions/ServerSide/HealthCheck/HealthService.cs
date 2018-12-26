using Grpc.Core;
using Grpc.Health.V1;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Grpc.Health.V1.Health;
using static Grpc.Health.V1.HealthCheckResponse.Types;

namespace Grpc.Extensions.ServerSide.HealthCheck
{
    public class HealthService : HealthBase
    {
        private readonly object myLock = new object();

        private readonly Dictionary<string, HealthCheckResponse.Types.ServingStatus> statusMap =
            new Dictionary<string, HealthCheckResponse.Types.ServingStatus>();

        /// <summary>
        /// Sets the health status for given service.
        /// </summary>
        /// <param name="service">The service. Cannot be null.</param>
        /// <param name="status">the health status</param>
        public void SetStatus(string service, HealthCheckResponse.Types.ServingStatus status)
        {
            lock (myLock)
            {
                statusMap[service] = status;
            }
        }

        /// <summary>
        /// Clears health status for given service.
        /// </summary>
        /// <param name="service">The service. Cannot be null.</param>
        public void ClearStatus(string service)
        {
            lock (myLock)
            {
                statusMap.Remove(service);
            }
        }

        /// <summary>
        /// Clears statuses for all services.
        /// </summary>
        public void ClearAll()
        {
            lock (myLock)
            {
                statusMap.Clear();
            }
        }

        /// <summary>
        /// Performs a health status check.
        /// </summary>
        /// <param name="request">The check request.</param>
        /// <param name="context">The call context.</param>
        /// <returns>The asynchronous response.</returns>
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            lock (myLock)
            {
                var service = request.Service;

                ServingStatus status;
                if (!statusMap.TryGetValue(service, out status))
                {
                    // 默认为健康状态
                    status = ServingStatus.Serving;
                }

                return Task.FromResult(new HealthCheckResponse { Status = status });
            }
        }
    }
}