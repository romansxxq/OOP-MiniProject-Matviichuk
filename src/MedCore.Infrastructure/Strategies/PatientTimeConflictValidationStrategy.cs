using MedCore.Domain.Entities;
using MedCore.Domain.Enums;
using MedCore.Domain.Interfaces;

namespace MedCore.Infrastructure.Strategies;

public class PatientTimeConflictValidationStrategy : IValidationStrategy
{
    public bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing)
    {
        return !existing.Any(a =>
            a.PatientId == app.PatientId &&
            a.AppointmentTime == app.AppointmentTime &&
            a.Status != AppointmentStatus.Cancelled);
    }
}
