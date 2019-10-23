using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Finder.Plugin.Demo
{
    public class TimestampBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Current Time V5: {DateTime.Now}");

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
