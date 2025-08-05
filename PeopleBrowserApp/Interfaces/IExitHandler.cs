namespace PeopleBrowserApp.Interfaces
{
    public interface IExitHandler
    {
        bool ShouldExit { get; }
        void RequestExit();
    }
}
