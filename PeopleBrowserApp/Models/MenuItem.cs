namespace PeopleBrowserApp.Models
{    
    public class MenuItem
    {
        public string? Key { get; set; }
        public string? Description { get; set; }
        public MenuItem(string? optionKey, string? description)
        {
            Key = optionKey;
            Description = description;
        }
    }
}
