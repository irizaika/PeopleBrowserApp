﻿namespace PeopleBrowserApp.Interfaces
{
    public interface IConsole
    {
        void WriteLine(string message);
        void Write(string message);
        string? ReadLine();
    }
}
