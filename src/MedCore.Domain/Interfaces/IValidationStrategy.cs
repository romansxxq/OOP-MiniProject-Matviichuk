using MedCore.Domain.Entities;
namespace MedCore.Domain.Interfaces;
/// <summary>
/// Defines a validation rule for appointments.
/// </summary>
public interface IValidationStrategy
{
    /// <summary>Returns true when the appointment passes the rule.</summary>
    bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing);
}