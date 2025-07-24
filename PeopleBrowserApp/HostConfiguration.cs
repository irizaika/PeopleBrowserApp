using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace PeopleBrowserApp
{
    internal static class HostConfiguration
    {
        const string appSettingsFileName = "appsettings.json";

        public static IHostBuilder ConfigureHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var appSettings = new AppSettings();
                    context.Configuration.GetSection("Api").Bind(appSettings);

                    if (string.IsNullOrWhiteSpace(appSettings.ApiBaseUrl))
                    {
                        Console.WriteLine(string.Format(Messages.SettingNotFound, "ApiBaseUrl", appSettingsFileName));
                    }

                    services.AddSingleton(appSettings);
                    services.AddSingleton<IPeopleRepository, PeopleRepository>();
                    services.AddSingleton<PeopleService>();
                    services.AddSingleton<Menu>();
                });
    }

}
