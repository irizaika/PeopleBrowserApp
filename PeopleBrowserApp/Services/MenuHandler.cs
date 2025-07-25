using PeopleBrowserApp.Configurations;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Resources;

namespace PeopleBrowserApp.Services
{
    public class MenuHandler
    {
        private readonly IPeopleService _peopleService;
        private readonly AppSettings _settings;
        private readonly IConsole _console;
        private readonly IConsoleCancelHandler _cancellation;
        private readonly IDisplayService _displyService;

        private readonly Dictionary<string, Func<CancellationToken, Task>> _menuActions;
        private bool _exit = false;

        public MenuHandler(IPeopleService peopleService, AppSettings settings, IConsole console,
            IConsoleCancelHandler cancellation, IDisplayService displayService)
        {
            _peopleService = peopleService;
            _settings = settings;
            _console = console;
            _cancellation = cancellation;
            _displyService = displayService;

            _menuActions = new()
            {
                ["1"] = ListPeople,
                ["2"] = SearchPeople,
                ["3"] = ViewPersonDetails,
                ["4"] = Exit
            };
        }

        public async Task ShowAsync()
        {
            _console.WriteLine($"People Directory from {_settings.ApiBaseUrl}\n");

            while (!_exit)
            {
                _displyService.DisplayMenuOptions();

                var input = ReadRequiredInput("");
                if (input == null)
                {
                    _console.WriteLine("No input provided. Exiting.");
                    break;
                }

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

        private string? ReadRequiredInput(string prompt)
        {
            _console.Write(prompt);
            var input = _console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                _console.WriteLine(Messages.EmptyValueTryAgain + Environment.NewLine);
                return null;
            }
            return input;
        }

        async Task ListPeople(CancellationToken cancellationToken)
        {
            _console.WriteLine("\nListing people...\n");

            var result = await _peopleService.ListPeopleAsync(cancellationToken);

            _displyService.DisplayPeople(result, Messages.ListIsEmpty);
        }

        async Task SearchPeople(CancellationToken cancellationToken)
        {
            var name = ReadRequiredInput("\nEnter name to search: ");

            if (name != null)
            {
                var result = await _peopleService.SearchPeopleAsync(name, cancellationToken);
                _displyService.DisplayPeople(result, Messages.NoResultsFound);
            }
        }

        async Task ViewPersonDetails(CancellationToken cancellationToken)
        {
            var username = ReadRequiredInput("\nEnter username: ");

            if (username != null)
            {
                var result = await _peopleService.GetPersonDetailsAsync(username, cancellationToken);
                _displyService.DisplayPerson(result, username);
            }
        }

        private Task Exit(CancellationToken _)
        {
            _exit = true;
            return Task.CompletedTask;
        }
    }
}
