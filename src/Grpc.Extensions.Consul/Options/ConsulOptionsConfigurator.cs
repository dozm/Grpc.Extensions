using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extensions.Consul.Options
{
    public class ConsulOptionsConfigurator : IConfigureOptions<ConsulOptions>
    {

        private readonly IConfiguration _config;

        public ConsulOptionsConfigurator(IConfiguration config)
        {
            _config = config;
        }

        public void Configure(ConsulOptions options)
        {
            var config = _config.GetSection("Consul");
            if (config == null)
                return;

            var address = config.GetSection("Address")?.Get<string>();
            if (address != null)
                options.Address = address;

            ConfigureServiceRegistration(options);
        }

        private void ConfigureServiceRegistration(ConsulOptions options)
        {
            var conifg = _config.GetSection("Consul:ServiceRegistration");
            if (conifg == null)
                return;

            if(options.ServiceRegistration == null)
            {
                options.ServiceRegistration = new ServiceRegistrationOptions();
            }

            var sro = options.ServiceRegistration;

            var consulServiceName = conifg["ConsulServiceName"];
            if (consulServiceName != null)
                sro.ConsulServiceName = consulServiceName;

            var enableTagOverride = conifg.GetSection("EnableTagOverride");
            if (enableTagOverride != null)
                sro.EnableTagOverride = enableTagOverride.Get<bool>();
        }
    }
}
