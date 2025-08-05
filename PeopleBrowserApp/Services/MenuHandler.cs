using PeopleBrowserApp.Configurations;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using PeopleBrowserApp.Resources;

namespace PeopleBrowserApp.Services
{
    public class MenuHandler
    {
        private readonly AppSettings _settings;
        private readonly IConsole _console;
        private readonly IConsoleCancelHandler _cancellation;
        private readonly IDisplayService _displayService;
        private readonly IExitHandler _exitHandler;

        private readonly Dictionary<string, Func<CancellationToken, Task>> _menuActions;
        private readonly IEnumerable<MenuItem> _menuItems;

        public MenuHandler(AppSettings settings, IConsole console,
            IConsoleCancelHandler cancellation, IDisplayService displayService,
            MenuActionsService menuActionsService, IExitHandler exitHandler)
        {
            _settings = settings;
            _console = console;
            _cancellation = cancellation;
            _displayService = displayService;
            _exitHandler = exitHandler;
            
            _menuActions = menuActionsService.MenuActions();
            _menuItems = menuActionsService.MenuItems();
        }

        public async Task ShowAsync()
        {
            _console.WriteLine($"People Directory from {_settings.ApiBaseUrl}\n");

            while (!_exitHandler.ShouldExit)
            {
                _displayService.DisplayMenuOptions(_menuItems);

                var input = _displayService.ReadRequiredInput("");

                using var cts = new CancellationTokenSource();
                using var reg = _cancellation.Register(() => cts.Cancel());


                if (input != null && _menuActions.TryGetValue(input, out var action))
                {
                    await action(cts.Token);
                }
                else
                {
                    _console.WriteLine(Messages.InvalidOption);
                    _console.ReadLine();
                }
            }

            _console.WriteLine("Goodbye!");
        }
    }
}
