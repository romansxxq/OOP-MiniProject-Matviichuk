namespace MedCore.Domain.Interfaces;

/// <summary>
/// Generic repository contract for basic CRUD operations.
/// </summary>
public interface IRepository<T, TId>
{
    /// <summary>Returns all entities.</summary>
    IReadOnlyCollection<T> GetAll();
    /// <summary>Returns an entity by id or null if missing.</summary>
    T? GetById(TId id);
    /// <summary>Adds a new entity.</summary>
    void Add(T entity);
    /// <summary>Updates an existing entity.</summary>
    void Update(T entity);
    /// <summary>Deletes an entity by id.</summary>
    void Delete(TId id);
}
