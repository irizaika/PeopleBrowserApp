using PeopleBrowserApp.Models;

namespace PeopleBrowserApp.Interfaces
{
    public interface IPeopleService
    {
        Task<IReadOnlyList<PersonDto>> ListPeopleAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<PersonDto>> SearchPeopleAsync(string name, CancellationToken cancellationToken);
        Task<PersonDto?> GetPersonDetailsAsync(string username, CancellationToken cancellationToken);
    }
}
