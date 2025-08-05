using Newtonsoft.Json.Linq;
using PeopleBrowserApp.Interfaces;

namespace PeopleBrowserApp.Tests.UnitTests.MenuTests.ConsoleHelpers
{
    public class TestConsoleMock : IConsole
    {
        private readonly Queue<string> _inputs = new();
        public List<string> Outputs { get; } = new();

        public int InputsConsumed { get; private set; } = 0;

        public TestConsoleMock(IEnumerable<string> inputs)
        {
            foreach (var input in inputs)
            {
                _inputs.Enqueue(input);
            }
        }

        public IReadOnlyList<string> Inputs => _inputs.ToList();

        public string? ReadLine()
        {
            if (_inputs.Count > 0)
            {
                //InputsConsumed++;
                //return _inputs.Dequeue();

                var input = _inputs.Dequeue();
                Outputs.Add(input);
                InputsConsumed++;
                return input;
            }

            return null;
        }

        public void Write(string value)
        {
            Outputs.Add(value);
        }

        public void WriteLine(string value = "")
        {
            Outputs.Add(value);
        }
    }
}
