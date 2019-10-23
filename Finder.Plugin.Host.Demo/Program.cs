using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Finder.Plugin.Abstractions;
using McMaster.NETCore.Plugins;

namespace Finder.Plugin.Host.Demo
{
    class Program
    {
        private static Dictionary<PluginLoader, IPlugin> _plugins = new Dictionary<PluginLoader, IPlugin>();

        static async Task Main(string[] args)
        {
            var pluginPath = args[0];
            var loader = PluginLoader.CreateFromAssemblyFile(pluginPath, new[] { typeof(IPlugin) }, config =>
            {
                config.EnableHotReload = true;
                config.IsUnloadable = true;
            });

            await StartPlugin(loader);

            loader.Reloaded += async (sender, eventArgs) =>
            {
                await ReloadedInfo(sender, eventArgs);
            };

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, __) => cts.Cancel();

            await Task.Delay(-1, cts.Token);
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


        private static async Task StopOldPlugin(PluginLoader loader)
        {
            await _plugins[loader].Stop();
            _plugins.Remove(loader);
        }

        private static async Task RestartPlugin(PluginLoader loader)
        {
            await StopOldPlugin(loader);
            await StartPlugin(loader);
        }
    }
}
