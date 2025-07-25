using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Services;

namespace PeopleBrowserApp.Tests.IntegrationTests
{
    [TestFixture]
    public class PeopleServiceIntegrationTests
    {
        private PeopleService _service;
        private IPeopleRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new InMemoryPeopleRepository();
            _service = new PeopleService(_repository);
        }

        [Test]
        public async Task ListPeopleAsync_ShouldReturnAllPeople()
        {
            var people = await _service.ListPeopleAsync(CancellationToken.None);

            Assert.That(people, Is.Not.Null);
            Assert.That(people, Has.Count.EqualTo(4));
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldReturnMatchingPeople()
        {
            var results = await _service.SearchPeopleAsync("John", CancellationToken.None);
            
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Any(p => p.FirstName == "John" && p.LastName == "Doe"), Is.True);
        }

        [Test]
        public async Task GetPersonDetailsAsync_ShouldReturnCorrectPerson()
        {
            var person = await _service.GetPersonDetailsAsync("asmith", CancellationToken.None);

            Assert.That(person, Is.Not.Null);
            Assert.That(person.FirstName + " " + person.LastName, Is.EqualTo("Alice Smith"));
        }


        [Test]
        public async Task GetPersonDetailsAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var person = await _service.GetPersonDetailsAsync("unknown", CancellationToken.None);

            Assert.That(person, Is.Null);
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldBeCaseInsensitive()
        {
            var results = await _service.SearchPeopleAsync("JOHN", CancellationToken.None);
            Assert.That(results.Any(p => p.FirstName == "John"), Is.True);
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldMatchByPartialFirstName()
        {
            var results = await _service.SearchPeopleAsync("Al", CancellationToken.None);
            Assert.Multiple(() =>
            {
                Assert.That(results.Any(p => p.FirstName.StartsWith("Al")), Is.True);
                Assert.That(results, Has.Count.EqualTo(2));
            });
        }
    }
}
