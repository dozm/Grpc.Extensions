﻿using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Linq;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Grpc.Extensions.Client
{
    public class ChannelProvider : IChannelProvider
    {
        private readonly GrpcClientOptions _options;
        private readonly ILoggerFactory _loggerFactory;
        private ConcurrentDictionary<string, IChannelPool> _channelPools = new ConcurrentDictionary<string, IChannelPool>();

        public ChannelProvider(IOptions<GrpcClientOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _loggerFactory = loggerFactory;
        }

        public Channel GetChannel(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentException($"Parameter {nameof(serviceName)} cannot be null.");

            var channelPool = _channelPools.GetOrAdd(serviceName, CreateChannelPool);
            var channel = channelPool.Rent();

            return channel;
        }

        public Channel GetChannel(Type clientType)
        {
            var serviceName = _options.Clients.FirstOrDefault(cm => cm.ClientType == clientType)?.ServiceName;

            return GetChannel(serviceName);
        }



        private IChannelPool CreateChannelPool(string serviceName)
        {
            if (!_options.ServicesEndpoint.TryGetValue(serviceName, out var serviceEndpoint))
            {
                throw new Exception($"Not found endpoint config of {serviceName}");
            }

            return new ChannelPool(serviceEndpoint, _loggerFactory.CreateLogger<ChannelPool>());
        }
    }
}
