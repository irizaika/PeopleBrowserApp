namespace PeopleBrowserApp.Services
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuOptionAttribute : Attribute
    {
        public string OptionKey { get; }
        public string Description { get; }

        public MenuOptionAttribute(string optionKey, string description)
        {
            OptionKey = optionKey;
            Description = description;
        }
    }
}
