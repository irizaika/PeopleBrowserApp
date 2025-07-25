using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using Trippin;

namespace PeopleBrowserApp.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IPeopleRepository _repository;

        public PeopleService(IPeopleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<PersonDto>> ListPeopleAsync(CancellationToken cancellationToken = default)
        {
            var people = await _repository.GetPeopleAsync(cancellationToken);
            return people.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<PersonDto>> SearchPeopleAsync(string firstName, CancellationToken cancellationToken = default)
        {
            var people = await _repository.SearchPeopleAsync(firstName, cancellationToken);
            return people.Select(MapToDto).ToList();
        }

        public async Task<PersonDto?> GetPersonDetailsAsync(string username, CancellationToken cancellationToken = default)
        {
            var person = await _repository.GetPersonAsync(username, cancellationToken);
            if (person == null)
                return null;
            return MapToDto(person);
        }

        private static PersonDto MapToDto(Person p)
        {
            return new PersonDto
            {
                UserName = p.UserName ?? string.Empty,
                FirstName = p.FirstName ?? string.Empty,
                LastName = p.LastName ?? string.Empty,
                Age = (int?)p.Age,

                Gender = p.Gender switch
                {
                    PersonGender.Male => Gender.Male,
                    PersonGender.Female => Gender.Female,
                    _ => Gender.Unknown
                },

                Emails = p.Emails?.ToList(),

                Features = p.Features?.Select(f => f switch
                {
                    Trippin.Feature.Feature1 => Models.Feature.Feature1,
                    Trippin.Feature.Feature2 => Models.Feature.Feature2,
                    Trippin.Feature.Feature3 => Models.Feature.Feature3,
                    Trippin.Feature.Feature4 => Models.Feature.Feature4,
                    _ => Models.Feature.Unknown
                }).ToList(),

                HomeAddress = !string.IsNullOrEmpty(p.HomeAddress?.Address)
                ? $"{p.HomeAddress.Address}, {p.HomeAddress.City?.Name}, {p.HomeAddress.City?.Region}, {p.HomeAddress.City?.CountryRegion}"
                : string.Empty,

                Addresses = p.AddressInfo?.Select(a =>
                    $"{a.Address}, {a.City?.Name}, {a.City?.Region}, {a.City?.CountryRegion}"
                    ).ToList() ?? new List<string>()

            };
        }

    }
}
 