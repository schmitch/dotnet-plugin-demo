using System;
using System.Threading.Tasks;

namespace Finder.Plugin.Abstractions
{
    public interface IPlugin
    {
        string GetName();
        Task Start();
        Task Stop();
    }
}
