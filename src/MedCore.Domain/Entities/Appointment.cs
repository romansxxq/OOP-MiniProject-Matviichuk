using MedCore.Domain.Enums;
namespace MedCore.Domain.Entities;
public class Appointment
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int DoctorId { get; private set; }
    public DateTime AppointmentTime { get; private set; }
    public AppointmentStatus Status { get; private set; }

    public Appointment(int id, int patientId, int doctorId, DateTime appointmentTime)
    {
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentTime = appointmentTime;
        Status = AppointmentStatus.New;
    }

    public void Cancel(AppointmentStatus newStatus)
    {
        Status = AppointmentStatus.Cancelled;
    }
}