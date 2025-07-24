using Moq;
using PeopleBrowserApp;
using Trippin;

[TestFixture]
public class PeopleServiceGetListTests
{
    private Mock<IPeopleRepository> _mockRepo;
    private PeopleService _service;
    private StringWriter _consoleOutput;
    private TextWriter _originalConsoleOut;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IPeopleRepository>();
        _service = new PeopleService(_mockRepo.Object);

        _originalConsoleOut = Console.Out;
        _consoleOutput = new StringWriter();
        Console.SetOut(_consoleOutput);
    }

    [TearDown]
    public void Teardown()
    {
        Console.SetOut(_originalConsoleOut);
        _consoleOutput.Dispose();
    }

    private List<Person> GetSamplePeople() => new()
    {
        new Person { FirstName = "Alice", LastName = "Wonderland", UserName = "alice" },
        new Person { FirstName = "Bob", LastName = "Builder", UserName = "bob" }
    };

    [Test]
    public async Task ListPeopleAsync_ShouldPrintFormattedOutput_WhenPeopleExist()
    {
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(GetSamplePeople());

        await _service.ListPeopleAsync();

        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("1. Alice Wonderland (alice)"));
        Assert.That(output, Does.Contain("2. Bob Builder (bob)"));
    }

    [Test]
    public async Task ListPeopleAsync_ShouldPrintNoResults_WhenListIsEmpty()
    {
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<Person>());

        await _service.ListPeopleAsync();

        Assert.That(_consoleOutput.ToString(), Does.Contain("No data found"));
    }

    [Test]
    public void ListPeopleAsync_ShouldThrow_WhenCancelled()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new TaskCanceledException());

        Assert.ThrowsAsync<TaskCanceledException>(() => _service.ListPeopleAsync(cts.Token));
    }

    [Test]
    public async Task ListPeopleAsync_ShouldNotThrow_WhenNotCancelled()
    {
        var people = GetSamplePeople().Take(1).ToList();
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(people);

        var cts = new CancellationTokenSource();
        await _service.ListPeopleAsync(cts.Token);

        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("Alice Wonderland (alice)"));
    }

    [Test]
    public async Task ListPeopleAsync_ShouldRespectCancellationToken()
    {
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .Returns(async (CancellationToken token) =>
                 {
                     await Task.Delay(5000, token);
                     return new List<Person>();
                 });

        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Will cancel before delay finishes

        Assert.ThrowsAsync<TaskCanceledException>(() => _service.ListPeopleAsync(cts.Token));
    }

    [Test]
    public async Task ListPeopleAsync_ShouldNotGetCancellationToken()
    {
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .Returns(async (CancellationToken token) =>
                 {
                     await Task.Delay(1000, token);
                     return GetSamplePeople();
                 });

        var cts = new CancellationTokenSource();
        cts.CancelAfter(3000); // Will cancel after delay finishes

        await _service.ListPeopleAsync(cts.Token);

        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("1. Alice Wonderland (alice)"));
        Assert.That(output, Does.Contain("2. Bob Builder (bob)"));
    }

    [Test]
    public async Task ListPeopleAsync_ShouldPrintOneFormattedOutput()
    {
        var people = GetSamplePeople().Take(1).ToList(); 
        _mockRepo.Setup(r => r.GetPeopleAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(people.Take(1));

        await _service.ListPeopleAsync();

        var output = _consoleOutput.ToString();
        Assert.That(output, Does.Contain("1. Alice Wonderland (alice)"));
        Assert.That(output, Does.Not.Contain("2. Bob Builder (bob)"));
    }
}
