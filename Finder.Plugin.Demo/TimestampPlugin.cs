using System;
using System.Threading.Tasks;
using Finder.Plugin.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finder.Plugin.Demo
{
    public class TimestampPlugin : IPlugin
    {
        private IHost _host;

        public string GetName()
        {
            return "Timestamp Plugin V5";
        }

        public async Task Start()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimestampBackgroundService>();
                })
                .Build();
            await _host.StartAsync();

            Console.WriteLine("Host Started");
        }

        public async Task Stop()
        {
            await _host.StopAsync();
            Console.WriteLine("Host Stopped");
        }
    }
}
