using MedCore.Domain.Entities;
namespace MedCore.Domain.Interfaces;
public interface IAppointmentRepository : IRepository<Appointment, int>
{
    IReadOnlyCollection<Appointment> GetByDoctorId(int doctorId);
    IReadOnlyCollection<Appointment> GetByPatientId(int patientId);
}