namespace PeopleBrowserApp.Interfaces
{
    public interface IConsoleCancelHandler
    {
        IDisposable Register(Action callback);
    }

}
