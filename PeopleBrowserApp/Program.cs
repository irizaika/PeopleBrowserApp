using Microsoft.Extensions.DependencyInjection;

namespace PeopleBrowserApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var host = HostConfiguration.ConfigureHost(args).Build();

            var menu = host.Services.GetRequiredService<Menu>();
            await menu.ShowAsync();
        }
    }
}
