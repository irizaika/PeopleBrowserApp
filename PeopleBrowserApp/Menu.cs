using System.Threading;
using System.Xml.Linq;

namespace PeopleBrowserApp
{
    public class Menu
    {
        private readonly PeopleService _peopleService;
        private readonly AppSettings _settings;

        public Menu(PeopleService peopleService, AppSettings settings)
        {
            _peopleService = peopleService;
            _settings = settings;
        }

        public async Task ShowAsync()
        {
            bool exit = false;
            Console.WriteLine($"People Directory from {_settings.ApiBaseUrl}\n");

            while (!exit)
            {
                Console.WriteLine("1. List all people");
                Console.WriteLine("2. Search people");
                Console.WriteLine("3. View person details");
                Console.WriteLine("4. Exit\n");
                Console.Write("Choose an option: ");

                var input = Console.ReadLine();

                using var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine(Environment.NewLine + Messages.OperationCanceled + Environment.NewLine);
                    cts.Cancel();
                    e.Cancel = true; // prevent app from exiting
                };

                try
                {
                    switch (input)
                    {
                        case "1":
                            await ListPeople(cts.Token);
                            break;
                        case "2":
                            await SearchPeople(cts.Token);
                            break;
                        case "3":
                            await ViewPersonDetails(cts.Token);
                            break;
                        case "4":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine(Messages.InvalidOption);
                            Console.ReadLine();
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The operation was cancelled by the user.\n");
                }
                Console.WriteLine();
            }

            Console.WriteLine("Goodbye!");
        }

        async Task ListPeople(CancellationToken cancellationToken)
        {
            Console.WriteLine("\nListing people...\n");
            await _peopleService.ListPeopleAsync(cancellationToken);
        }

        async Task SearchPeople(CancellationToken cancellationToken)
        {
            Console.Write("\nEnter name to search: ");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine(Messages.EmptyValueTryAgain + Environment.NewLine);
                return;
            }

            await _peopleService.SearchPeopleAsync(name, cancellationToken);
        }

        async Task ViewPersonDetails(CancellationToken cancellationToken)
        {
            Console.Write("\nEnter username: ");
            var username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine(Messages.EmptyValueTryAgain + Environment.NewLine);
                return;
            }

            await _peopleService.GetPersonDetailsAsync(username, cancellationToken);
        }
    }
}
