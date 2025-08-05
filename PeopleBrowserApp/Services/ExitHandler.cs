using PeopleBrowserApp.Interfaces;

namespace PeopleBrowserApp.Services
{
    public class ExitHandler : IExitHandler
    {
        private bool _shouldExit;

        public bool ShouldExit => _shouldExit;

        public void RequestExit()
        {
            _shouldExit = true;
        }
    }
}
