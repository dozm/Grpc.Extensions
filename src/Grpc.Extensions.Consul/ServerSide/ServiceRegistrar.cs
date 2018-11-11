using Consul;
using Grpc.Extensions.ServerSide;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Grpc.Extensions.ExecutionStrategies;
using Microsoft.Extensions.Hosting;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class ServiceRegistrar : IHostedService //IGrpcServerLifetime
    {
        private bool _registerSuccessfully;
        private readonly IConsulClientFactory _consulClientFactory;
        private readonly IAgentServiceRegistrationFactory _registrationFactory;
        private readonly IExecutionStrategyFactory _executionStrategyFactory;
        private readonly ConsulOptions _options;
        private readonly ILogger<ServiceRegistrar> _logger;
        private readonly IGrpcServerContextAccessor _grpcServerContextAccessor;
        private AgentServiceRegistration _serviceRegistration;

        private GrpcServerContext Context => _grpcServerContextAccessor.Context;

        public ServiceRegistrar(IConsulClientFactory consulClientFactory,
            IAgentServiceRegistrationFactory registrationFactory,
            IExecutionStrategyFactory executionStrategyFactory,
            IOptions<ConsulOptions> options,
            ILogger<ServiceRegistrar> logger,
            IGrpcServerContextAccessor grpcServerContextAccessor)
        {
            _consulClientFactory = consulClientFactory;
            _registrationFactory = registrationFactory;
            _executionStrategyFactory = executionStrategyFactory;
            _options = options.Value;
            _logger = logger;
            _grpcServerContextAccessor = grpcServerContextAccessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            // TODO: 是否需要注册所有已监听的端口。
            _serviceRegistration = _registrationFactory.Create(Context);

            //_registerSuccessfully = await RetryingRegisterServiceAsync(_registration, cancellationToken);

            //if (_registerSuccessfully)
            {
                TryStartTTL();
            }

        }

        private async Task<bool> RetryingRegisterServiceAsync(AgentServiceRegistration registration, CancellationToken cancellationToken)
        {
            return await _executionStrategyFactory.Create().ExecuteAsync(registration, RegisterAsync, cancellationToken);
        }

        private async Task<bool> RegisterAsync(AgentServiceRegistration registration, CancellationToken cancellationToken)
        {

            var consul = _consulClientFactory.Create();
            try
            {
                _logger.LogInformation($"正在向 Consul 注册服务:{registration.ID}");
                await consul.Agent.ServiceRegister(registration, cancellationToken);
                _logger.LogInformation($"已将服务注册到 Consul:{registration.ID}");
                return true;
            }
            finally
            {
                consul.Dispose();
            }
        }

        private void TryStartTTL()
        {
            // TODO: 支持多个 TTL chek
            var ttlCheck = FindTTLCheck(_serviceRegistration);
            if (ttlCheck != null)
            {
                var ttlTimer = new Timer(async _ => await PassTTL(ttlCheck), null, 0, 10000);

                var ttlTask = PassTTL(ttlCheck);
            }

        }

        private async Task PassTTL(AgentCheckRegistration ttlCheck, CancellationToken cancellationToken = default)
        {
            var consul = _consulClientFactory.Create();
            try
            {
                //await client.Agent.PassTTL(ttlCheck.ID, $"Time:{DateTime.Now}", cancellationToken);
                _logger.LogDebug($"pass ttl {ttlCheck.ID}");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                consul.Dispose();
            }
        }

        private async Task DeregisterAsync(AgentServiceRegistration registration, CancellationToken cancellationToken = default)
        {
            //if (!_registerSuccessfully)
            //    return;

            using (var consul = _consulClientFactory.Create())
            {
                // 与服务关联的Check也将随之注销。
                await consul.Agent.ServiceDeregister(registration.ID, cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await DeregisterAsync(_serviceRegistration, cancellationToken);
        }

        private static AgentCheckRegistration FindTTLCheck(AgentServiceRegistration registration)
        {
            var ttlCheck = registration.Check?.TTL != null ? registration.Check : null;
            if (ttlCheck == null)
                ttlCheck = registration.Checks?.FirstOrDefault(c => c.TTL != null);

            return ttlCheck as AgentCheckRegistration;
        }
    }
}
