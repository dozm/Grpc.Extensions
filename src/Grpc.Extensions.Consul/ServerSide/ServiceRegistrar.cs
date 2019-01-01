using Consul;
using Grpc.Extensions.ExecutionStrategies;
using Grpc.Extensions.ServerSide;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.Consul.ServerSide
{
    public class ServiceRegistrar : IHostedService
    {
        private Timer _ttlTimer;
        private AgentServiceRegistration _serviceRegistration;
        private ServiceEntry _serviceRegistered;

        private readonly IConsulClientFactory _consulClientFactory;
        private readonly IAgentServiceRegistrationFactory _registrationFactory;
        private readonly IExecutionStrategyFactory _executionStrategyFactory;
        private readonly ConsulOptions _options;
        private readonly ILogger<ServiceRegistrar> _logger;
        private readonly IGrpcServerContextAccessor _grpcServerContextAccessor;

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

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            // 非阻塞启动
            var registerServiceTask = StartRegisterAsync(cancellationToken);

            await Task.CompletedTask;
        }

        protected virtual async Task StartRegisterAsync(CancellationToken cancellationToken = default)
        {
            _serviceRegistration = _registrationFactory.Create();
            _serviceRegistered = await RegisterAsync(_serviceRegistration, cancellationToken);

            TryStartTTL(_serviceRegistered, _serviceRegistration);
        }

        protected virtual async Task<ServiceEntry> RegisterAsync(AgentServiceRegistration serviceReg, CancellationToken cancellationToken = default)
        {
            return await _executionStrategyFactory.Create().ExecuteAsync(serviceReg, RegisterImplementAsync, cancellationToken);
        }

        protected virtual async Task<ServiceEntry> RegisterImplementAsync(AgentServiceRegistration serviceReg, CancellationToken cancellationToken = default)
        {
            var consul = _consulClientFactory.Create();
            try
            {
                _logger.LogInformation($"正在向 Consul 注册服务:{serviceReg.ID}");
                await consul.Agent.ServiceRegister(serviceReg, cancellationToken);
                var serviceRegistered = (await consul.Health.Service(serviceReg.Name)).Response.FirstOrDefault(s => s.Service.ID == serviceReg.ID);
                _logger.LogInformation($"已将服务注册到 Consul:{serviceReg.ID}");

                return serviceRegistered;
            }
            finally
            {
                consul.Dispose();
            }
        }

        protected virtual void TryStartTTL(ServiceEntry serviceRegistered, AgentServiceRegistration serviceRegistration)
        {
            var ttlCheck = FindTTLCheck(serviceRegistration);

            if (ttlCheck != null)
            {
                var service = serviceRegistered.Service;
                var ttlCheckId = _serviceRegistered.Checks.FirstOrDefault(c => c.Name == ttlCheck.Name)?.CheckID;

                _ttlTimer = new Timer(async _ => await PassTTL(ttlCheckId, service.ID, service.Service), null, 0, (int)ttlCheck.TTL.Value.TotalMilliseconds);
            }
        }

        protected virtual async Task PassTTL(string ttlCheckId, string serviceId, string serviceName, CancellationToken cancellationToken = default)
        {
            var consul = _consulClientFactory.Create();

            try
            {
                var note = $"TTL passing:{DateTime.Now}";
                await consul.Agent.PassTTL(ttlCheckId, note, cancellationToken).ConfigureAwait(false);
                _logger.LogDebug(note);
            }
            catch (ConsulRequestException ex)
            {
                _logger.LogError(ex, $"TTL 时发生异常，TTL check id:{ttlCheckId}");
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.InternalServerError:
                        var queryResult = await consul.Health.Service(serviceName).ConfigureAwait(false);
                        var service = queryResult.Response.FirstOrDefault(s => s.Service.ID == serviceId);
                        if (service == null || service.Checks.FirstOrDefault(c => c.CheckID == ttlCheckId) == null)
                        {
                            _logger.LogInformation("开始尝试重新注册服务...");
                            await ReRegisterAsync(_serviceRegistration);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"TTL 时发生异常，TTL check id:{ttlCheckId}");
            }
            finally
            {
                consul.Dispose();
            }
        }

        protected virtual async Task ReRegisterAsync(AgentServiceRegistration registration, CancellationToken cancellationToken = default)
        {
            if (_ttlTimer != null)
            {
                _ttlTimer.Dispose();
                _ttlTimer = null;
            }
            if (_serviceRegistered != null)
            {
                await DeregisterAsync(_serviceRegistered.Service.ID);
                _serviceRegistered = null;
            }
            _serviceRegistered = await RegisterAsync(registration);
            TryStartTTL(_serviceRegistered, registration);
        }

        protected virtual async Task DeregisterAsync(string serviceId, CancellationToken cancellationToken = default)
        {
            var consul = _consulClientFactory.Create();
            try
            {
                _logger.LogInformation($"正在注销服务：{serviceId}");
                await consul.Agent.ServiceDeregister(serviceId, cancellationToken);
                _logger.LogInformation($"服务已注销：{serviceId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从 Consul 注销服务时发生异常。");
            }
            finally
            {
                consul.Dispose();
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _ttlTimer?.Dispose();
            if (_serviceRegistered != null)
            {
                await DeregisterAsync(_serviceRegistered.Service.ID, cancellationToken);
            }
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