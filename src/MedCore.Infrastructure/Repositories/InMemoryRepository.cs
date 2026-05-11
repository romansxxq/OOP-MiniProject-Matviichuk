using MedCore.Domain.Abstractions;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Repositories;

public class InMemoryRepository<T, TId> : IRepository<T, TId>
    where T : IEntity<TId>
    where TId : notnull
{
    private readonly Dictionary<TId, T> _storage = new();

    public IReadOnlyCollection<T> GetAll()
    {
        return _storage.Values.ToList();
    }

    public T? GetById(TId id)
    {
        return _storage.TryGetValue(id, out var entity) ? entity : default;
    }

    public void Add(T entity)
    {
        if (_storage.ContainsKey(entity.Id))
            throw new InvalidOperationException("Entity with the same id already exists.");

        _storage[entity.Id] = entity;
    }

    public void Update(T entity)
    {
        if (!_storage.ContainsKey(entity.Id))
            throw new InvalidOperationException("Entity not found for update.");

        _storage[entity.Id] = entity;
    }

    public void Delete(TId id)
    {
        _storage.Remove(id);
    }
}
