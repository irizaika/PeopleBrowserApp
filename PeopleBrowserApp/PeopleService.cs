using Trippin;


namespace PeopleBrowserApp
{
    public class PeopleService
    {

        private readonly IPeopleRepository _repository;

        public PeopleService(IPeopleRepository repository)
        {
            _repository = repository;
        }

        public async Task ListPeopleAsync(CancellationToken cancellationToken = default)
        {
            var people = await _repository.GetPeopleAsync(cancellationToken);
            DisplayPeople(people.ToList(), Messages.ListIsEmpty);
        }

        public async Task SearchPeopleAsync(string firstName, CancellationToken cancellationToken = default)
        {
            var people = await _repository.SearchPeopleAsync(firstName, cancellationToken);
            DisplayPeople(people.ToList(), Messages.NoResultsFound);
        }

        public async Task GetPersonDetailsAsync(string username, CancellationToken cancellationToken = default)
        {
            var person = await _repository.GetPersonAsync(username, cancellationToken);
            if (person == null)
            {
                Console.WriteLine(string.Format(Messages.UsernameNotFound, username));
                return;
            }
            DisplayPerson(person);
        }


        private void DisplayPeople(List<Person> people, string emptyMessage)
        {
            if (people.Any())
            {
                int i = 1;
                foreach (var person in people)
                {
                    Console.WriteLine($"{i++}. {person.FirstName} {person.LastName} ({person.UserName})");
                }
            }
            else
            {
                Console.WriteLine(emptyMessage);
            }
        }

        private void DisplayPerson(Person person)
        {
            Console.WriteLine($"Name: {person.FirstName} {person.MiddleName ?? ""} {person.LastName}");
            Console.WriteLine($"Gender: {person.Gender}");
            Console.WriteLine($"Age: {(person.Age.HasValue ? person.Age.ToString() : "N/A")}");
            Console.WriteLine("Emails: " + (person.Emails?.Count > 0 ? string.Join(", ", person.Emails) : "None"));
            Console.WriteLine("Features: " + (person.Features?.Count > 0 ? string.Join(", ", person.Features) : "None"));
            Console.WriteLine($"Favorite Feature: {person.FavoriteFeature}");

            if (person.AddressInfo != null && person.AddressInfo.Count > 0)
            {
                Console.WriteLine("Addresses:");
                foreach (var addr in person.AddressInfo)
                {
                    Console.WriteLine($"  {addr.Address}, {addr.City?.Name}, {addr.City?.Region}, {addr.City?.CountryRegion}");
                }
            }
            else
            {
                Console.WriteLine("Addresses: None");
            }

            if (person.HomeAddress != null && !string.IsNullOrEmpty(person.HomeAddress.Address))
            {
                Console.WriteLine("Home Address:"); 
                Console.WriteLine($"  {person.HomeAddress.Address}, {person.HomeAddress.City?.Name}, {person.HomeAddress.City?.Region}, {person.HomeAddress.City?.CountryRegion}");
            }
            else
            {
                Console.WriteLine("Home Address: None");
            }

            Console.WriteLine("\n");
        }

    }
}
