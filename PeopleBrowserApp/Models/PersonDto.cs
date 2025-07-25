namespace PeopleBrowserApp.Models
{
    public class PersonDto
    {
        public string UserName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";
        public int? Age { get; set; }
        public Gender Gender { get; set; }
        public IReadOnlyList<string>? Emails { get; set; }
        public IReadOnlyList<Feature>? Features { get; set; }
        public string HomeAddress { get; set; } = "";
        public List<string> Addresses { get; set; } = new List<string> { "" };
    }
}
