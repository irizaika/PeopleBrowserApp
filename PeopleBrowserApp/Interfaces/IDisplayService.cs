using PeopleBrowserApp.Models;

namespace PeopleBrowserApp.Interfaces
{
    public interface IDisplayService
    {
        void DisplayMenuOptions(IEnumerable<MenuItem> menuItems);
        void DisplayPeople(IReadOnlyList<PersonDto> people, string emptyMessage);
        void DisplayPerson(PersonDto? person, string username);
        string? ReadRequiredInput(string prompt);
    }
}
