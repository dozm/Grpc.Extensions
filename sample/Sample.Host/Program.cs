using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Sample.Host
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var host = BuildHost(args))
            {
                host.Run();
            }
        }

        private static IHost BuildHost(string[] args)
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, "configs");
            var host = new HostBuilder()
                .ConfigureHostConfiguration(b =>
                {
                    b.SetBasePath(configPath)
                    .AddEnvironmentVariables("DOTNET_")
                    .AddJsonFile("hostsettings.json", optional: true)
                    ;
                })
                .ConfigureAppConfiguration((ctx, b) =>
                {
                    b.SetBasePath(configPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", true, true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(ctx.Configuration.GetValue<int?>("HostShutdownTimeout", null) ?? 20));

                    var startup = new Startup(ctx);
                    startup.ConfigureServices(services);
                })
                .ConfigureLogging((ctx, b) =>
                {
                    b.AddConfiguration(ctx.Configuration.GetSection("Logging"))

                    .AddConsole()
                 ;
                })
                .UseConsoleLifetime()
                .Build();

            return host;
        }
    }
}