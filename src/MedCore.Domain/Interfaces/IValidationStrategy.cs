using MedCore.Domain.Entities;
namespace MedCore.Domain.Interfaces;
public interface IValidationStrategy
{
    bool IsValid(Appointment app, IReadOnlyCollection<Appointment> existing);
}