using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Finder.Plugin.Host.Demo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<PluginRegistryHostedService>();
                })
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
    }
}
