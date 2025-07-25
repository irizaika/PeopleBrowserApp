using PeopleBrowserApp.Models;

namespace PeopleBrowserApp.Interfaces
{
    public interface IDisplayService
    {
        void DisplayMenuOptions();
        void DisplayPeople(IReadOnlyList<PersonDto> people, string emptyMessage);
        void DisplayPerson(PersonDto? person, string username);
    }
}
