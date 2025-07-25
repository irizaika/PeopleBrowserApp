using PeopleBrowserApp.Interfaces;

namespace PeopleBrowserApp.Services
{
    public class ConsoleCancelHandler : IConsoleCancelHandler
    {
        public IDisposable Register(Action handler)
        {
            ConsoleCancelEventHandler eventHandler = (_, e) =>
            {
                e.Cancel = true; // Prevent the process from terminating
                handler();
            };

            Console.CancelKeyPress += eventHandler;

            return new Unregister(() =>
            {
                Console.CancelKeyPress -= eventHandler;
            });
        }

        private class Unregister : IDisposable
        {
            private readonly Action _unregister;
            private bool _disposed;

            public Unregister(Action unregister)
            {
                _unregister = unregister;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _unregister();
                    _disposed = true;
                }
            }
        }
    }

}
