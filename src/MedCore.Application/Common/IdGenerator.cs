using MedCore.Domain.Abstractions;
using MedCore.Domain.Interfaces;

namespace MedCore.Application.Common;

public static class IdGenerator
{
    public static int NextId<T>(IRepository<T, int> repository) where T : IEntity<int>
    {
        var next = repository.GetAll()
            .Select(entity => entity.Id)
            .DefaultIfEmpty(0)
            .Max();

        return next + 1;
    }
}
