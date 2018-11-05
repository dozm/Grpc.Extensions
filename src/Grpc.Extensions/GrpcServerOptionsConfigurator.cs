using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using Grpc.Core;
using System.IO;

namespace Grpc.Extensions
{
    public class GrpcServerOptionsConfigurator : IConfigureOptions<GrpcServerOptions>
    {
        readonly private IConfiguration _config;
        public GrpcServerOptionsConfigurator(IConfiguration config)
        {
            _config = config;
        }
        public void Configure(GrpcServerOptions options)
        {
            var grpcServiceConfig = _config.GetSection("GrpcServerOptions");

            var serverPosts = grpcServiceConfig.GetSection("ServerPorts").Get<List<ServerPortConfig>>();

            serverPosts?.ForEach(c => options.AddPort(c.Host ?? "[::]", c.Port ?? ServerPort.PickUnused, CreateCredentials(c.Credentials)));

        }

        private ServerCredentials CreateCredentials(SslServerCredentialsConfig credConfig)
        {
            if (credConfig == null)
                return ServerCredentials.Insecure;

            var rootCert = File.ReadAllText(credConfig.RootCertFile);

            var keyCertificatePairs = credConfig.KeyCertPairs.Select(
                c => new KeyCertificatePair(File.ReadAllText(c.CertChainFile), File.ReadAllText(c.PrivateKeyFile)));

            SslServerCredentials creds;

            if (credConfig.ClientCertRequestType != null)
            {
                creds = new SslServerCredentials(keyCertificatePairs, rootCert, (SslClientCertificateRequestType)credConfig.ClientCertRequestType);
            }
            else
            {
                creds = new SslServerCredentials(keyCertificatePairs, rootCert, credConfig.ForceClientAuth ?? false);
            }

            return creds;

        }

        private class ServerPortConfig
        {
            public string Host { get; set; }
            public int? Port { get; set; }

            public SslServerCredentialsConfig Credentials { get; set; }
        }

        private class SslServerCredentialsConfig
        {
            public string RootCertFile { get; set; }
            public KeyCertificatePairConfig[] KeyCertPairs { get; set; }
            public bool? ForceClientAuth { get; set; }
            public SslClientCertificateRequestType? ClientCertRequestType { get; set; }
        }

        private class KeyCertificatePairConfig
        {
            public string CertChainFile { get; set; }

            public string PrivateKeyFile { get; set; }
        }
    }
}
