using PeopleBrowserApp.Interfaces;

namespace PeopleBrowserApp.Tests.UnitTests.MenuTests.ConsoleHelpers
{
    public class CancellationMock : IConsoleCancelHandler
    {
        private Action? _handler;

        public IDisposable Register(Action handler)
        {
            _handler = handler;
            return new Disposable(() => _handler = null);
        }

        public void Trigger() => _handler?.Invoke();

        private class Disposable : IDisposable
        {
            private readonly Action _dispose;

            public Disposable(Action dispose) => _dispose = dispose;

            public void Dispose() => _dispose();
        }
    }

}
