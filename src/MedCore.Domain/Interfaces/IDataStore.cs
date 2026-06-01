namespace MedCore.Domain.Interfaces;

/// <summary>
/// Abstraction for loading and saving application data.
/// </summary>
public interface IDataStore<T>
{
    /// <summary>Loads data from storage.</summary>
    Task<T> LoadAsync(CancellationToken cancellationToken = default);
    /// <summary>Saves data to storage.</summary>
    Task SaveAsync(T data, CancellationToken cancellationToken = default);
}
