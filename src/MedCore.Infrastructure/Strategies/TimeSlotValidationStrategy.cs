using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Strategies;

public class TimeSlotValidationStrategy : IValidationStrategy
{
    public bool IsValid(Appointment app, List<Appointment> existing)
    {
        return !existing.Any(a => a.AppointmentTime == app.AppointmentTime);
    }
}