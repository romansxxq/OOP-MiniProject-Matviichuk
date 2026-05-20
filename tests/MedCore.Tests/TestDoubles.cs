using MedCore.Application.Common;
using MedCore.Application.Models;
using MedCore.Domain.Interfaces;

namespace MedCore.Tests;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateTime now)
    {
        Now = now;
    }

    public DateTime Now { get; }
}

internal sealed class RecordingErrorReporter : IErrorReporter
{
    public List<(string Message, Exception? Exception)> Entries { get; } = new();

    public void Report(string message, Exception? exception = null)
    {
        Entries.Add((message, exception));
    }
}

internal sealed class ThrowingDataStore : IDataStore<MedCoreData>
{
    private readonly Exception? _loadException;
    private readonly Exception? _saveException;

    public ThrowingDataStore(Exception? loadException = null, Exception? saveException = null)
    {
        _loadException = loadException;
        _saveException = saveException;
    }

    public Task<MedCoreData> LoadAsync(CancellationToken cancellationToken = default)
    {
        return _loadException is null
            ? Task.FromResult(new MedCoreData())
            : Task.FromException<MedCoreData>(_loadException);
    }

    public Task SaveAsync(MedCoreData data, CancellationToken cancellationToken = default)
    {
        return _saveException is null
            ? Task.CompletedTask
            : Task.FromException(_saveException);
    }
}
