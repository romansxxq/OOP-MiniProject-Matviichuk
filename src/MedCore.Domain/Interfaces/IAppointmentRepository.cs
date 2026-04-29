using MedCore.Domain.Entities;
namespace MedCore.Domain.Interfaces;
public interface IAppointmentRepository
{
    void Add(Appointment appointment);
    List<Appointment> GetAll();
    List<Appointment> GetByDoctorId(int doctorId);
}