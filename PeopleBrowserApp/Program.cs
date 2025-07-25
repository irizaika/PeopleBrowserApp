using Microsoft.Extensions.DependencyInjection;
using PeopleBrowserApp.Configurations;
using PeopleBrowserApp.Services;

namespace PeopleBrowserApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var host = HostConfiguration.ConfigureHost(args).Build();

            var menu = host.Services.GetRequiredService<MenuHandler>();
            await menu.ShowAsync();
        }
    }
}
