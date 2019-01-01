using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extensions.ClientSide
{
    public class LoadBalancer : ILoadBalancer
    {
        private readonly AsyncLock _asyncLock = new AsyncLock();

        private DateTime _lastUpdateTime;
        private readonly int _updateIntervalSeconds = 10;

        private ChannelSet _channelCache = new ChannelSet();

        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IChannelFactory _channelFactory;
        private readonly ILogger<LoadBalancer> _logger;

        public string ServiceName { get; }

        private TaskCompletionSource<bool> _updateChannelsTcs;

        public LoadBalancer(string serviceName, IServiceDiscovery serviceDiscovery, IChannelFactory channelFactory, ILoggerFactory loggerFactory)
        {
            ServiceName = serviceName;
            _serviceDiscovery = serviceDiscovery;
            _channelFactory = channelFactory;
            _logger = loggerFactory.CreateLogger<LoadBalancer>();
        }

        public Channel SelectChannel()
        {
            return SelectChannelAsync().GetAwaiter().GetResult();
        }

        public async Task<Channel> SelectChannelAsync()
        {
            var channlSet = _channelCache;
            Channel channel = null;
            if (channlSet.HasChannel())
            {
                channel = channlSet.Next().Channel;
                if ((DateTime.Now - _lastUpdateTime).TotalSeconds > _updateIntervalSeconds)
                {
                    var task = TryUpdateAsync();
                }

                return channel;
            }
            else
            {
                await TryUpdateAsync();
                channlSet = _channelCache;

                if (channlSet.HasChannel())
                {
                    return channlSet.Next().Channel;
                }
                else
                {
                    throw new Exception($"没有可用的channel");
                }
            }
        }

        public Task<bool> TryUpdateAsync()
        {
            var tcs = _updateChannelsTcs;

            if (tcs == null)
            {
                using (_asyncLock.Lock())
                {
                    tcs = _updateChannelsTcs;
                    if (tcs == null)
                    {
                        _updateChannelsTcs = tcs = new TaskCompletionSource<bool>();
                        var task = new Task(async () =>
                        {
                            try
                            {
                                await Update();
                                tcs.SetResult(true);
                            }
                            catch (Exception ex)
                            {
                                tcs.SetException(new Exception("更新Channel时发生异常。", ex));
                                _logger.LogError(ex, ex.Message);
                            }
                            finally
                            {
                                _updateChannelsTcs = null;
                            }
                        });
                        task.Start();
                    }
                }
            }

            return tcs.Task;
        }

        private async Task Update()
        {
            var newEndpoints = (await DiscoveryEndPoints()).Distinct();
            var origChannels = _channelCache;
            var newCount = newEndpoints.Count();
            var newChannels = new List<ChannelEntry>();

            var reusable = origChannels.Where(c => newEndpoints.Contains(c.EndPoint) && c.Channel.State != ChannelState.Shutdown);
            var toRelease = origChannels.Except(reusable);

            var shouldCreate = newEndpoints.Where(ep => !reusable.Any(c => c.EndPoint.Equals(ep)));

            foreach (var i in shouldCreate)
            {
                newChannels.Add(CreateChannelEntry(i));
            }
            newChannels.AddRange(reusable);

            foreach (var i in toRelease)
            {
                i.Dispose();
            }

            _channelCache = new ChannelSet(newChannels);
            _lastUpdateTime = DateTime.Now;
        }

        private async Task<ServiceEndPoint[]> DiscoveryEndPoints(CancellationToken cancellationToken = default)
        {
            var endpoints = await _serviceDiscovery.DiscoverAsync(ServiceName, cancellationToken);
            if (endpoints.Length == 0)
            {
                throw new Exception($"未发现可用的服务:{ServiceName}");
            }

            return endpoints;
        }

        private ChannelEntry CreateChannelEntry(ServiceEndPoint serviceEndPoint)
        {
            return new ChannelEntry(_channelFactory.Create(serviceEndPoint), serviceEndPoint);
        }
    }
}