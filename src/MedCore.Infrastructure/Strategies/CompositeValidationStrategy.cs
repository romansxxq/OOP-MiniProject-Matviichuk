using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Strategies;

public class CompositeValidationStrategy : IValidationStrategy
{
    private readonly IReadOnlyCollection<IValidationStrategy> _strategies;

    public CompositeValidationStrategy(IEnumerable<IValidationStrategy> strategies)
    {
        _strategies = strategies.ToList();
    }

    public bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing)
    {
        return _strategies.All(strategy => strategy.IsValid(app, existing));
    }
}
