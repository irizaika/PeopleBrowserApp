using Moq;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Services;
using Trippin;

namespace PeopleBrowserApp.Tests.UnitTests.PeopleServiceTests
{
    [TestFixture]
    public class SearchTests
    {
        private Mock<IPeopleRepository> _mockRepo;
        private PeopleService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPeopleRepository>();
            _service = new PeopleService(_mockRepo.Object);
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldReturnMatchingPeople()
        {
            var people = new List<Person>
            {
                new() { FirstName = "Alice", LastName = "Wonderland", UserName = "alice" },
                new() { FirstName = "Calum", LastName = "Smith", UserName = "calumsmith" }
            };

            _mockRepo.Setup(r => r.SearchPeopleAsync("al", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(people);

            var result = await _service.SearchPeopleAsync("al");

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result.Any(p => p.UserName == "alice"), Is.True);
                Assert.That(result.Any(p => p.UserName == "calumsmith"), Is.True);
            });
        }

        [Test]
        public async Task SearchPeopleAsync_ShouldReturnEmptyList_WhenNoMatch()
        {
            _mockRepo.Setup(r => r.SearchPeopleAsync("xyz", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Person>());

            var result = await _service.SearchPeopleAsync("xyz");

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void SearchPeopleAsync_ShouldThrowTaskCanceledException_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockRepo.Setup(r => r.SearchPeopleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new TaskCanceledException());

            Assert.ThrowsAsync<TaskCanceledException>(() =>
                _service.SearchPeopleAsync("test", cts.Token));
        }
    }
}
