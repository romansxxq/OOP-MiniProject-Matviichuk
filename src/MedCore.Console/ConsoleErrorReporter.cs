using MedCore.Application.Common;

namespace MedCore.Console;

public sealed class ConsoleErrorReporter : IErrorReporter
{
    public void Report(string message, Exception? exception = null)
    {
        if (exception is null)
        {
            Console.Error.WriteLine(message);
            return;
        }

        Console.Error.WriteLine($"{message} ({exception.GetType().Name}: {exception.Message})");
    }
}
