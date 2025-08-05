using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using PeopleBrowserApp.Resources;
using System.Reflection;

namespace PeopleBrowserApp.Services
{
    public class MenuActionsService
    {
        private readonly IPeopleService _peopleService;
        private readonly IConsole _console;
        private readonly IDisplayService _displayService;
        private readonly IExitHandler _exitHandler;

        private readonly Dictionary<string, Func<CancellationToken, Task>> _menuActions;
        private IEnumerable<MenuItem> _menuItems;

        public MenuActionsService(IPeopleService peopleService, IConsole console,
            IDisplayService displayService, IExitHandler exitHandler)
        {
            _peopleService = peopleService;
            _console = console;
            _displayService = displayService;
            _exitHandler = exitHandler;

            var methods = this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<MenuOptionAttribute>() != null &&
                            m.ReturnType == typeof(Task) &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType == typeof(CancellationToken));

            _menuActions = methods
                .Select(m =>
                {
                    var attr = m.GetCustomAttribute<MenuOptionAttribute>()!;
                    var action = (Func<CancellationToken, Task>)Delegate.CreateDelegate(
                        typeof(Func<CancellationToken, Task>), this, m);
                    return new { attr.OptionKey, Action = action };
                })
                .ToDictionary(x => x.OptionKey, x => x.Action);


            _menuItems = _menuActions.Select(kvp =>
            {
                var method = kvp.Value.Method;
                var attr = method.GetCustomAttribute<MenuOptionAttribute>();
                return new MenuItem(attr?.OptionKey, attr?.Description);
            });
            _exitHandler = exitHandler;
        }

        public Dictionary<string, Func<CancellationToken, Task>> MenuActions()
        {
            return _menuActions;
        }

        public IEnumerable<MenuItem> MenuItems()
        {
            return _menuItems;
        }

        [MenuOption("1", "List People")]
        async Task ListPeople(CancellationToken cancellationToken)
        {
            _console.WriteLine("\nListing people...\n");

            var result = await _peopleService.ListPeopleAsync(cancellationToken);

            _displayService.DisplayPeople(result, Messages.ListIsEmpty);
        }

        [MenuOption("2", "Search People")]
        async Task SearchPeople(CancellationToken cancellationToken)
        {
            var name = _displayService.ReadRequiredInput("\nEnter name to search: ");

            if (name != null)
            {
                var result = await _peopleService.SearchPeopleAsync(name, cancellationToken);
                _displayService.DisplayPeople(result, Messages.NoResultsFound);
            }
        }

        [MenuOption("3", "View person details")]
        async Task ViewPersonDetails(CancellationToken cancellationToken)
        {
            var username = _displayService.ReadRequiredInput("\nEnter username: ");

            if (username != null)
            {
                var result = await _peopleService.GetPersonDetailsAsync(username, cancellationToken);
                _displayService.DisplayPerson(result, username);
            }
        }

        [MenuOption("4", "Exit")]
        private Task Exit(CancellationToken _)
        {
            // _exit = true;
            _exitHandler.RequestExit();
            return Task.CompletedTask;
        }
    }
}
