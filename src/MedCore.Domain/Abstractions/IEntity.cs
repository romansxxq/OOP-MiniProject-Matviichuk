namespace MedCore.Domain.Abstractions;

public interface IEntity<TId>
{
    TId Id { get; }
}
