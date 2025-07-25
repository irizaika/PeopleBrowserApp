using PeopleBrowserApp.Interfaces;

namespace PeopleBrowserApp.Tests.UnitTests.MenuTests.ConsoleHelpers
{
    public class TestConsoleMock : IConsole
    {
        private readonly Queue<string> _inputs;
        public readonly List<string> Outputs = [];

        public TestConsoleMock(IEnumerable<string> inputs)
        {
            _inputs = new Queue<string>(inputs);
        }

        public void WriteLine(string message) => Outputs.Add(message);
        public void Write(string message) => Outputs.Add(message);
        // public string? ReadLine() => _inputs.Count > 0 ? _inputs.Dequeue() : null;

        private int _readCount = 0;
        private const int MaxReadLimit = 100;

        public string? ReadLine()
        {
            _readCount++;
            if (_readCount > MaxReadLimit)
                throw new InvalidOperationException("Exceeded maximum allowed input reads (infinite loop protection).");

            return _inputs.Count > 0 ? _inputs.Dequeue() : null;
        }

    }
}
