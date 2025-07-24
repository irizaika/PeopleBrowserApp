using Microsoft.OData.Client;
using Trippin;

namespace PeopleBrowserApp
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly AppSettings _settings;
        private readonly Container _context;

        public PeopleRepository(AppSettings appSettings)
        {
            _settings = appSettings;
            _context = new Container(new Uri(_settings.ApiBaseUrl));
        }

        public async Task<IEnumerable<Person>> GetPeopleAsync(CancellationToken cancellationToken = default)
        {
            var people = await _context.People.ExecuteAsync(cancellationToken);
            return people.ToList();
        }

        public async Task<IEnumerable<Person>> SearchPeopleAsync(string firstName, CancellationToken cancellationToken = default)
        {
            string filter = $"contains(tolower(FirstName), '{firstName.ToLower()}')";
            var query = _context.People.AddQueryOption("$filter", filter);
            var people = await query.ExecuteAsync(cancellationToken);
            return people.ToList();
        }

        public async Task<Person?> GetPersonAsync(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                var person = await _context.People
                    .ByKey(username)
                    .GetValueAsync(cancellationToken);
                return person;
            }
            catch (DataServiceQueryException)
            {
                return null;
            }
        }
    }
}
