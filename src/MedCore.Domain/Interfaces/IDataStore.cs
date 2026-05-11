namespace MedCore.Domain.Interfaces;

public interface IDataStore<T>
{
    Task<T> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(T data, CancellationToken cancellationToken = default);
}
