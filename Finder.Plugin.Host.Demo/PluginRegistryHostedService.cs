using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finder.Plugin.Abstractions;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.Hosting;

namespace Finder.Plugin.Host.Demo
{
    public class PluginRegistryHostedService : BackgroundService
    {
        private const string PluginPath = "/Users/schmitch/projects/envisia/finder/Finder.Plugins/bin/plugins/Finder.Plugin.Demo/Finder.Plugin.Demo.dll";
        private static Dictionary<PluginLoader, IPlugin> _plugins = new Dictionary<PluginLoader, IPlugin>();

        public async Task Load(CancellationToken stoppingToken)
        {
            var loader = PluginLoader.CreateFromAssemblyFile(PluginPath, true, new[] { typeof(IPlugin) }, config =>
            {
                config.EnableHotReload = true;
                config.IsUnloadable = true;
            });

            await StartPlugin(loader);

            loader.Reloaded += async (sender, eventArgs) =>
            {
                await ReloadedInfo(sender, eventArgs);
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Load(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping Plugins");
            foreach (var kv in _plugins)
            {
                await StopOldPlugin(kv.Key, true);
            }

            Console.WriteLine("Stopped Plugins");

            _plugins.Clear();

            await base.StopAsync(cancellationToken);
            Console.WriteLine("Canceled");
        }

        private static async Task ReloadedInfo(object sender, PluginReloadedEventArgs eventArgs)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Reload Triggered, EventArgs: {eventArgs.Loader} / {sender}");
            Console.ResetColor();
            await RestartPlugin(eventArgs.Loader);
        }

        private static async Task StartPlugin(PluginLoader loader)
        {
            var pluginType = loader
                .LoadDefaultAssembly()
                .GetTypes()
                .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
            if (pluginType == null)
            {
                Console.WriteLine("No Plugin Found");
                return;
            }

            var plugin = Activator.CreateInstance(pluginType) as IPlugin;
            if (plugin == null)
            {
                Console.WriteLine("No Plugin Found");
                return;
            }

            _plugins.Add(loader, plugin);

            Console.WriteLine($"Created plugin instance '{plugin?.GetName()}'.");

            await plugin.Start();
        }


        private static async Task StopOldPlugin(PluginLoader loader, bool dispose = false)
        {
            await _plugins[loader].Stop();
            var removed = _plugins.Remove(loader);
            if (!removed)
            {
                Console.WriteLine("Error Removing Loader");
            }

            if (dispose)
            {
                loader.Dispose();
                Console.WriteLine("Loader Disposed");
            }
        }

        private static async Task RestartPlugin(PluginLoader loader)
        {
            await StopOldPlugin(loader);
            await StartPlugin(loader);
        }
    }
}
