using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
using MedCore.Infrastructure.Strategies;
using Xunit;

namespace MedCore.Tests;

public class ValidationStrategyTests
{
    [Fact]
    public void DoctorDailyLimitStrategy_ShouldReject_WhenLimitReached()
    {
        var strategy = new DoctorDailyLimitValidationStrategy(2);
        var doctorId = 10;
        var day = new DateTime(2026, 2, 1, 10, 0, 0);

        var existing = new List<Appointment>
        {
            new Appointment(1, 1, doctorId, day),
            new Appointment(2, 2, doctorId, day.AddHours(1))
        };

        var newAppointment = new Appointment(3, 3, doctorId, day.AddHours(2));

        Assert.False(strategy.IsValid(newAppointment, existing));
    }

    [Fact]
    public void CompositeValidationStrategy_ShouldReturnFalse_WhenAnyStrategyFails()
    {
        var strategies = new IValidationStrategy[]
        {
            new AlwaysValidStrategy(),
            new AlwaysInvalidStrategy()
        };

        var composite = new CompositeValidationStrategy(strategies);
        var appointment = new Appointment(1, 1, 1, DateTime.Now.AddDays(1));

        Assert.False(composite.IsValid(appointment, new List<Appointment>()));
    }

    private sealed class AlwaysValidStrategy : IValidationStrategy
    {
        public bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing) => true;
    }

    private sealed class AlwaysInvalidStrategy : IValidationStrategy
    {
        public bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing) => false;
    }
}
