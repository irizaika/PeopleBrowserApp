using Moq;
using PeopleBrowserApp.Configurations;
using PeopleBrowserApp.Interfaces;
using PeopleBrowserApp.Models;
using PeopleBrowserApp.Services;
using PeopleBrowserApp.Tests.UnitTests.MenuTests.ConsoleHelpers;
using Trippin;

namespace PeopleBrowserApp.Tests.UnitTests.MenuTests
{
    [TestFixture]
    public class ConsoleInputTests
    {
        private Mock<IPeopleService> _mockService;
        private CancellationMock _mockCancellation;
        private Mock<IExitHandler> _mockExitHandler;
        private MenuActionsService _menuActionsService;
        private DisplayService _displayService;

        private TestConsoleMock _testConsole;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IPeopleService>();
            _mockCancellation = new CancellationMock();
            _mockExitHandler = new Mock<IExitHandler>();
        }

        private MenuHandler SetupMenuHandler(TestConsoleMock console)
        {
            _testConsole = console;

            _displayService = new DisplayService(console);

            _mockExitHandler.Setup(e => e.ShouldExit)
                .Returns(() => _testConsole.InputsConsumed >= _testConsole.Inputs.Count);

            _menuActionsService = new MenuActionsService(
                _mockService.Object,
                _testConsole,
                _displayService,
                _mockExitHandler.Object
            );

            return new MenuHandler(
                new AppSettings { ApiBaseUrl = "http://test" },
                _testConsole,
                _mockCancellation,
                _displayService,
                _menuActionsService,
                _mockExitHandler.Object
            );
        }

        [Test]
        public async Task Menu_ShouldShowInvalidOption_WhenInputIsUnknown()
        {
            var console = new TestConsoleMock(["99", "4"]); // "" to simulate pressing Enter after invalid option
            var menu = SetupMenuHandler(console);
            await menu.ShowAsync();

            foreach (var output in console.Outputs)
                Console.WriteLine($"Console output: {output}");

            Assert.Multiple(() =>
            {
                Assert.That(console.Outputs.Any(o => o.Contains("Invalid option")), Is.True);
                Assert.That(console.Outputs.Last(), Is.EqualTo("Goodbye!"));
            });
        }

        [Test]
        public async Task Menu_ShouldCallListPeople_WhenOption1Selected()
        {
            _mockService.Setup(s => s.ListPeopleAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<PersonDto>());

            var console = new TestConsoleMock(["1", "4"]);
            var menu = SetupMenuHandler(console);
            await menu.ShowAsync();

            foreach (var output in console.Outputs)
                Console.WriteLine($"Console output: {output}");

            _mockService.Verify(s => s.ListPeopleAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(console.Outputs.Last(), Is.EqualTo("Goodbye!"));
        }

        [Test]
        public async Task Menu_ShouldCallSearchPeople_WhenOption2AndValidInputProvided()
        {
            _mockService.Setup(s => s.SearchPeopleAsync("John", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<PersonDto>());

            var console = new TestConsoleMock(["2", "John", "4"]);
            var menu = SetupMenuHandler(console);
            await menu.ShowAsync();

            foreach (var output in console.Outputs)
                Console.WriteLine($"Console output: {output}");

            _mockService.Verify(s => s.SearchPeopleAsync("John", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Menu_ShouldNotCallSearchPeople_WhenInputIsEmpty()
        {
            var console = new TestConsoleMock(["2", "   ", "4"]);
            var menu = SetupMenuHandler(console);
            await menu.ShowAsync();

            foreach (var output in console.Outputs)
                Console.WriteLine($"Console output: {output}");


            _mockService.Verify(s => s.SearchPeopleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.That(console.Outputs.Any(o => o.Contains("Value cannot be empty. Please try again")), Is.True);
        }

        [Test]
        public async Task Menu_ShouldCallGetPersonDetails_WhenOption3AndUsernameProvided()
        {
            _mockService.Setup(s => s.GetPersonDetailsAsync("jdoe", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new PersonDto());

            var console = new TestConsoleMock(["3", "jdoe", "4"]);
            var menu = SetupMenuHandler(console);
            await menu.ShowAsync();

            foreach (var output in console.Outputs)
                Console.WriteLine($"Console output: {output}");

            _mockService.Verify(s => s.GetPersonDetailsAsync("jdoe", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
