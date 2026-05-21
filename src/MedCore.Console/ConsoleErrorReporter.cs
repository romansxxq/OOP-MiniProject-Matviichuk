using MedCore.Application.Common;

namespace MedCore.Console;

public sealed class ConsoleErrorReporter : IErrorReporter
{
    public void Report(string message, Exception? exception = null)
    {
        if (exception is null)
        {
            System.Console.Error.WriteLine(message);
            return;
        }

        System.Console.Error.WriteLine($"{message} ({exception.GetType().Name}: {exception.Message})");
    }
}
