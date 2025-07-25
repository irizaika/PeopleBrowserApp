using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using PeopleBrowserApp.Resources;
using Trippin;

namespace PeopleBrowserApp.Services
{
    public class DisplayService : IDisplayService
    {
        private readonly IConsole _console;

        public DisplayService(IConsole console)
        {
            _console = console;
        }

        public void DisplayMenuOptions()
        {
            _console.WriteLine("1. List all people");
            _console.WriteLine("2. Search people");
            _console.WriteLine("3. View person details");
            _console.WriteLine("4. Exit\n");
            _console.Write("Choose an option: ");
        }


        public void DisplayPeople(IReadOnlyList<PersonDto> people, string emptyMessage)
        {
            if (people.Count > 0)
            {
                int i = 1;
                foreach (var person in people)
                {
                    _console.WriteLine($"{i++}. {person.FirstName} {person.LastName} ({person.UserName})");
                }
            }
            else
            {
                _console.WriteLine(emptyMessage);
            }
            _console.WriteLine("\n");
        }

        public void DisplayPerson(PersonDto? person, string username)
        {
            if (person != null)
            {
                _console.WriteLine($"Name: {person.FullName}");
                _console.WriteLine($"Gender: {person.Gender}");
                _console.WriteLine($"Age: {(person.Age.HasValue ? person.Age.ToString() : "N/A")}");
                _console.WriteLine("Emails: " + (person.Emails?.Any() == true ? string.Join(", ", person.Emails) : "None"));
                _console.WriteLine("Features: " + (person.Features?.Any() == true ? string.Join(", ", person.Features) : "None"));
                _console.WriteLine("Home Address: " + (!string.IsNullOrEmpty(person.HomeAddress) ? person.HomeAddress : "None"));

                if (person.Addresses != null && person.Addresses.Count > 0)
                {
                    _console.WriteLine("Addresses:");
                    foreach (var addr in person.Addresses)
                    {
                        _console.WriteLine($"  {addr}");
                    }
                }
                else
                {
                    _console.WriteLine("Addresses: None");
                }
            }
            else
            {
                _console.WriteLine(string.Format(Messages.UsernameNotFound, username));
            }

            _console.WriteLine("\n");
        }
    }

}
