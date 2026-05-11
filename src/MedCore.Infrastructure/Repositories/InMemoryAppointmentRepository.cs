using MedCore.Domain.Entities;
using MedCore.Domain.Interfaces;
namespace MedCore.Infrastructure.Repositories;
public class InMemoryAppointmentRepository : InMemoryRepository<Appointment, int>, IAppointmentRepository
{
    public IReadOnlyCollection<Appointment> GetByDoctorId(int id)
    {
        return GetAll().Where(a => a.DoctorId == id).ToList();
    }

    public IReadOnlyCollection<Appointment> GetByPatientId(int patientId)
    {
        return GetAll().Where(a => a.PatientId == patientId).ToList();
    }
}