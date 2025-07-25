using PeopleBrowserApp.Interfaces;
using Trippin;

namespace PeopleBrowserApp.Tests.IntegrationTests
{
    public class InMemoryPeopleRepository : IPeopleRepository
    {
        private readonly List<Person> _people;

        public InMemoryPeopleRepository()
        {
            _people =
            [
                new() { UserName = "jdoe", FirstName = "John", LastName = "Doe" },
                new() { UserName = "asmith", FirstName = "Alice", LastName = "Smith" },
                new() { UserName = "bjones", FirstName = "Bob", LastName = "Jones" },
                new() { FirstName = "Calum", LastName = "Smith", UserName = "calumsmith" }
            ];
        }

        public Task<IEnumerable<Person>> GetPeopleAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_people.Select(p=>p));
        }

        public Task<IEnumerable<Person>> SearchPeopleAsync(string name, CancellationToken cancellationToken)
        {
            var results = _people.Where(p => p.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(results);
        }

        public Task<Person?> GetPersonAsync(string username, CancellationToken cancellationToken = default)
        {
            var person = _people.FirstOrDefault(p => p.UserName == username);
            return Task.FromResult(person);
        }
    }

}
