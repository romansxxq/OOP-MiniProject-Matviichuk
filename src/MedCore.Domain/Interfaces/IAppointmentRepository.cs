using MedCore.Domain.Entities;
namespace MedCore.Domain.Interfaces;
/// <summary>
/// Repository contract for appointment-specific queries.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment, int>
{
    /// <summary>Returns appointments for the given doctor.</summary>
    IReadOnlyCollection<Appointment> GetByDoctorId(int doctorId);
    /// <summary>Returns appointments for the given patient.</summary>
    IReadOnlyCollection<Appointment> GetByPatientId(int patientId);
}