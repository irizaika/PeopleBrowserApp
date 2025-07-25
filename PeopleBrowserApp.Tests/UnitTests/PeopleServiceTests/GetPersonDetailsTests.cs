using Moq;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using PeopleBrowserApp.Services;
using Trippin;

namespace PeopleBrowserApp.Tests.UnitTests.PeopleServiceTests
{
    [TestFixture]
    public class PeopleServiceDetailsTests
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
        public async Task GetPersonDetailsAsync_ShouldReturnCorrectDetails_WhenPersonExists()
        {
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Gender = PersonGender.Male,
                Age = 30,
                Emails = ["john@example.com"],
                Features = [Trippin.Feature.Feature1]
            };

            _mockRepo.Setup(r => r.GetPersonAsync("johndoe", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(person);

            var result = await _service.GetPersonDetailsAsync("johndoe");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.UserName, Is.EqualTo("johndoe"));
                Assert.That(result.FirstName, Is.EqualTo("John"));
                Assert.That(result.LastName, Is.EqualTo("Doe"));
                Assert.That(result.Gender, Is.EqualTo(Gender.Male));
                Assert.That(result.Age, Is.EqualTo(30));
                Assert.That(result.Emails?.FirstOrDefault(), Is.EqualTo("john@example.com"));
                Assert.That(result.Features?.FirstOrDefault(), Is.EqualTo(Models.Feature.Feature1));
            });
        }

        [Test]
        public async Task GetPersonDetailsAsync_ShouldReturnNull_WhenPersonNotFound()
        {
            _mockRepo.Setup(r => r.GetPersonAsync("notfound", It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Person?)null);

            var result = await _service.GetPersonDetailsAsync("notfound");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetPersonDetailsAsync_ShouldThrowTaskCanceledException_WhenCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockRepo.Setup(r => r.GetPersonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new TaskCanceledException());

            Assert.ThrowsAsync<TaskCanceledException>(() =>
                _service.GetPersonDetailsAsync("test", cts.Token));
        }

        [Test]
        public async Task GetPersonDetailsAsync_ShouldHandleNullPropertiesGracefully()
        {
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Gender = PersonGender.Male,
                Age = null,
                Emails = null,
                Features = null,
                AddressInfo = null,
                HomeAddress = null
            };

            _mockRepo.Setup(r => r.GetPersonAsync("johndoe", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(person);

            var result = await _service.GetPersonDetailsAsync("johndoe");

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Age, Is.Null);
                Assert.That(result.Emails, Is.Null);
                Assert.That(result.Features, Is.Null);
                Assert.That(result.Addresses, Is.Empty);
                Assert.That(result.HomeAddress, Is.Empty);
            });
        }
    }
}