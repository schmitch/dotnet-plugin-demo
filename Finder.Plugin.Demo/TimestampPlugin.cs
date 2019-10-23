using System.Threading.Tasks;
using Finder.Plugin.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Finder.Plugin.Demo
{
    public class TimestampPlugin : IPlugin
    {
        private IHost _host;

        public string GetName()
        {
            return "Timestamp Plugin V4";
        }

        public async Task Start()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimestampBackgroundService>();
                })
                .Build();
            await _host.StartAsync();
        }

        public async Task Stop()
        {
            await _host.StopAsync();
        }
    }
}
