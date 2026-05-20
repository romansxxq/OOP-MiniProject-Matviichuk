namespace MedCore.Application.Common;

public interface IErrorReporter
{
    void Report(string message, Exception? exception = null);
}

public sealed class NullErrorReporter : IErrorReporter
{
    public void Report(string message, Exception? exception = null)
    {
    }
}
