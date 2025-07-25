using Trippin;

namespace PeopleBrowserApp.Interfaces
{
    public interface IPeopleRepository
    {
        Task<IEnumerable<Person>> GetPeopleAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Person>> SearchPeopleAsync(string firstName, CancellationToken cancellationToken = default);
        Task<Person?> GetPersonAsync(string username, CancellationToken cancellationToken = default);
    }
}
