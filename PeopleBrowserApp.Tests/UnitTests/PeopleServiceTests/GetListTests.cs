using Moq;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Services;
using Trippin;

namespace PeopleBrowserApp.Tests.UnitTests.PeopleServiceTests
{
    [TestFixture]
    public class GetListTests
    {
        private Mock<IPeopleRepository> _mockRepo;
        private PeopleService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPeopleRepository>();
            _service = new PeopleService(_mockRepo.Object);
        }

        private static List<Person> SamplePeople => new()
        {
            new Person { FirstName = "Alice", LastName = "Wonderland", UserName = "alice" },
            new Person { FirstName = "Bob", LastName = "Builder", UserName = "bob" }
        };

        [Test]
        public async Task ListPeopleAsync_ShouldReturnAllPeople_WhenAvailable()
        {
            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(SamplePeople);

            var result = await _service.ListPeopleAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].UserName, Is.EqualTo("alice"));
                Assert.That(result[1].UserName, Is.EqualTo("bob"));
            });
        }

        [Test]
        public async Task ListPeopleAsync_ShouldReturnEmptyList_WhenNoPeopleExist()
        {
            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync([]);

            var result = await _service.ListPeopleAsync();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task ListPeopleAsync_ShouldReturnSinglePerson_WhenOnlyOneInRepo()
        {
            var onePerson = SamplePeople.Take(1).ToList();
            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(onePerson);

            var result = await _service.ListPeopleAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result[0].FirstName, Is.EqualTo("Alice"));
                Assert.That(result.Any(p => p.FirstName == "Bob"), Is.False);
            });
        }

        [Test]
        public void ListPeopleAsync_ShouldThrowTaskCanceledException_WhenCancelledImmediately()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new TaskCanceledException());

            Assert.ThrowsAsync<TaskCanceledException>(() => _service.ListPeopleAsync(cts.Token));
        }

        [Test]
        public async Task ListPeopleAsync_ShouldThrowTaskCanceledException_WhenCancelledMidway()
        {
            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .Returns(async (CancellationToken token) =>
                     {
                         await Task.Delay(5000, token);
                         return [];
                     });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            Assert.ThrowsAsync<TaskCanceledException>(() => _service.ListPeopleAsync(cts.Token));
        }

        [Test]
        public async Task ListPeopleAsync_ShouldReturnData_WhenNotCancelledPrematurely()
        {
            _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                     .Returns(async (CancellationToken token) =>
                     {
                         await Task.Delay(500, token); // Simulate delay
                         return SamplePeople;
                     });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(3000); // Cancel after enough time

            var result = await _service.ListPeopleAsync(cts.Token);

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(p => p.UserName == "alice"), Is.True);
        }
    }
}
